using Dalamud.Game.Command;
using Dalamud.Plugin;

namespace FlashOnReadyCheck;

public sealed class Plugin : IDalamudPlugin
{
    private const string TestFlashingCommand = "/readyflash";

    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Svc>();
        ReadyCheckHooking.Init();
        ReadyCheckHooking.AddOnReadyCheckStart(() => PluginFunctions.ExecuteFlashWindow(false));

        Svc.CommandManager.AddHandler(TestFlashingCommand, new CommandInfo(PluginFunctions.OnTestFlashingCommand)
        {
            HelpMessage = "Tries to flash the window after three seconds."
        });
    }

    public void Dispose()
    {
        Svc.CommandManager.RemoveHandler(TestFlashingCommand);
        ReadyCheckHooking.Uninit();
    }
}
