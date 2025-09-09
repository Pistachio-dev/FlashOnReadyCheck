using System;
using System.Runtime.InteropServices;

using Dalamud.Game;
using Dalamud.Hooking;
using Dalamud.Logging;

using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;


namespace FlashOnReadyCheck
{
    public static class ReadyCheckHooking
    {
        private static IntPtr mfpOnReadyCheckInitiated = IntPtr.Zero;
        private static Hook<ReadyCheckFuncDelegate> mReadyCheckInitiatedHook;

        private static IntPtr mfpOnReadyCheckEnd = IntPtr.Zero;
        private static Hook<ReadyCheckFuncDelegate> mReadyCheckEndHook;

        private delegate void ReadyCheckFuncDelegate(IntPtr ptr);

        public static event EventHandler OnReadyCheckInitiated;
        public static event EventHandler OnReadyCheckComplete;

        public static void Init(SigScanner sigScanner)
        {
            if (sigScanner == null)
            {
                throw new Exception("Error in \"MemoryHandler.Init()\": A null SigScanner was passed!");
            }

            //	Get Function Pointers, etc.
            try
            {
                //	When a ready check has been initiated by anyone.
                mfpOnReadyCheckInitiated = sigScanner.ScanText("40 ?? 48 83 ?? ?? 48 8B ?? E8 ?? ?? ?? ?? 48 ?? ?? ?? 33 C0 ?? 89");
                if (mfpOnReadyCheckInitiated != IntPtr.Zero)
                {
                    // Unless I add <TargetFramework>net6.0-windows</TargetFramework> in the .csproj, FromAddress is not found
                    mReadyCheckInitiatedHook = Hook<ReadyCheckFuncDelegate>.FromAddress(mfpOnReadyCheckInitiated, ReadyCheckInitiatedDetour);
                    mReadyCheckInitiatedHook.Enable();
                }

                //	When a ready check has been completed and processed.
                mfpOnReadyCheckEnd = sigScanner.ScanText("40 ?? 53 48 ?? ?? ?? ?? 48 81 ?? ?? ?? ?? ?? 48 8B ?? ?? ?? ?? ?? 48 33 ?? ?? 89 ?? ?? ?? 83 ?? ?? ?? 48 8B ?? 75 ?? 48");
                if (mfpOnReadyCheckEnd != IntPtr.Zero)
                {
                    mReadyCheckEndHook = Hook<ReadyCheckFuncDelegate>.FromAddress(mfpOnReadyCheckEnd, ReadyCheckEndDetour);
                    mReadyCheckEndHook.Enable();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error in \"MemoryHandler.Init()\" while searching for required function signatures; this probably means that the plugin needs to be updated due to changes in Final Fantasy XIV.  Raw exception as follows:\r\n{e}");
            }
        }

        private static void ReadyCheckInitiatedDetour(IntPtr ptr)
        {
            mReadyCheckInitiatedHook.Original(ptr);
            OnReadyCheckInitiated?.Invoke(null, EventArgs.Empty);
        }

        private static void ReadyCheckEndDetour(IntPtr ptr)
        {
            mReadyCheckEndHook.Original(ptr);
            OnReadyCheckComplete?.Invoke(null, EventArgs.Empty);
        }

        public static void Dispose()
        {
            mReadyCheckInitiatedHook?.Disable();
            mReadyCheckEndHook?.Disable();
            mReadyCheckInitiatedHook?.Dispose();
            mReadyCheckEndHook?.Dispose();
            mReadyCheckInitiatedHook = null;
            mReadyCheckEndHook = null;
        }
    }
}
