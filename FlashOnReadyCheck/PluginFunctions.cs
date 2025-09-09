using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FlashOnReadyCheck
{
    internal static class PluginFunctions
    {
        internal static void OnTestFlashingCommand(string command, string args)
        {
            Svc.Chat.Print($"FlashOnReadyCheck will try to flash the icon in 3 seconds. " +
                    $"Unless you alt-tab out, nothing will happen.");
            Svc.Framework.RunOnTick(() => ExecuteFlashWindow(true), TimeSpan.FromSeconds(3));
        }

        internal static void ExecuteFlashWindow(bool test)
        {
            if (FlashWindow.ApplicationIsActivated())
            {
                if (test) { Svc.Chat.Print("The game window was active, so, no flashing"); }
                return;
            }
            if (test) { Svc.Chat.Print("Flashing window now."); }
            var flashInfo = new FlashWindow.FLASHWINFO
            {
                cbSize = (uint)Marshal.SizeOf<FlashWindow.FLASHWINFO>(),
                uCount = uint.MaxValue,
                dwTimeout = 0,
                dwFlags = FlashWindow.FLASHW_ALL | FlashWindow.FLASHW_TIMERNOFG,
                hwnd = Process.GetCurrentProcess().MainWindowHandle,
            };
            FlashWindow.Flash(flashInfo);
        }
    }
}
