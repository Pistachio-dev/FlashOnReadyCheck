// Most copied from https://github.com/Infiziert90/ReadyCheckHelper
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using System;

namespace FlashOnReadyCheck
{
    public static class ReadyCheckHooking
    {
        private static IntPtr mfpOnReadyCheckInitiated = IntPtr.Zero;
        private static Hook<ReadyCheckFuncDelegate> mReadyCheckInitiatedHook;

        private static IntPtr mfpOnReadyCheckEnd = IntPtr.Zero;
        private static Hook<ReadyCheckFuncDelegate> mReadyCheckEndHook;

        private delegate void ReadyCheckFuncDelegate(IntPtr ptr);

        //	Events
        public static event EventHandler ReadyCheckInitiatedEvent;

        public static event EventHandler ReadyCheckCompleteEvent;

        private static Hook<AgentReadyCheck.Delegates.InitiateReadyCheck> MReadyCheckInitiatedHook;
        private static Hook<AgentReadyCheck.Delegates.EndReadyCheck> MReadyCheckEndHook;

        public static void AddOnReadyCheckStart(Action action)
        {
            ReadyCheckInitiatedEvent += (caller, args) => action();
        }

        public static unsafe void Init()
        {
            try
            {
                MReadyCheckInitiatedHook = Svc.Hook.HookFromAddress<AgentReadyCheck.Delegates.InitiateReadyCheck>(AgentReadyCheck.MemberFunctionPointers.InitiateReadyCheck, ReadyCheckInitiatedDetour);
                MReadyCheckInitiatedHook.Enable();

                MReadyCheckEndHook = Svc.Hook.HookFromAddress<AgentReadyCheck.Delegates.EndReadyCheck>(AgentReadyCheck.MemberFunctionPointers.EndReadyCheck, ReadyCheckEndDetour);
                MReadyCheckEndHook.Enable();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while searching for required function signatures; this probably means that the plugin needs to be updated due to changes in Final Fantasy XIV.\n{ex}");
            }
        }

        public static void Uninit()
        {
            MReadyCheckInitiatedHook?.Dispose();
            MReadyCheckEndHook?.Dispose();
        }

        private static unsafe void ReadyCheckInitiatedDetour(AgentReadyCheck* ptr)
        {
            MReadyCheckInitiatedHook.Original(ptr);
            ReadyCheckInitiatedEvent?.Invoke(null, EventArgs.Empty);
        }

        private static unsafe void ReadyCheckEndDetour(AgentReadyCheck* ptr)
        {
            MReadyCheckEndHook.Original(ptr);

            //	Update our copy of the data one last time.
            ReadyCheckCompleteEvent?.Invoke(null, EventArgs.Empty);
        }
    }
}
