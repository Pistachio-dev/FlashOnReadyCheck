using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FlashOnReadyCheck;

public sealed class Plugin : IDalamudPlugin
{


    private const string TestFlashingCommand = "/readyflash";

    public Plugin(IDalamudPluginInterface pluginInterface, SigScanner sigScanner)
    {
        pluginInterface.Create<Svc>();
        ReadyCheckHooking.Init(sigScanner);

        Svc.CommandManager.AddHandler(TestFlashingCommand, new CommandInfo(OnTestFlashingCommand)
        {
            HelpMessage = "Tries to flash the window after three seconds."
        });

    }

    public void Dispose()
    {
        Svc.CommandManager.RemoveHandler(TestFlashingCommand);
        ReadyCheckHooking.Dispose();
    }

    private void OnTestFlashingCommand(string command, string args)
    {
        Svc.Chat.Print($"FlashOnReadyCheck will try to flash the icon in 3 seconds. " +
                $"Unless you alt-tab out, nothing will happen.");
        Svc.Framework.RunOnTick(() => ExecuteFlashWindow(true), TimeSpan.FromSeconds(3));
    }

    private void ExecuteFlashWindow(bool test)
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
