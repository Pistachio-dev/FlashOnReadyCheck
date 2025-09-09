using Dalamud.Game.Command;
using Dalamud.Plugin;

namespace FlashOnReadyCheck;

public sealed class Plugin : IDalamudPlugin
{


    private const string TestFlashingCommand = "/readyflash";

    public Configuration Configuration { get; init; }

    public Plugin()
    {
        Configuration = Svc.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        Svc.CommandManager.AddHandler(TestFlashingCommand, new CommandInfo(OnTestFlashingCommand)
        {
            HelpMessage = "Tries to flash the window after three seconds."
        });
    }

    public void Dispose()
    {
        Svc.CommandManager.RemoveHandler(TestFlashingCommand);
    }

    private void OnTestFlashingCommand(string command, string args)
    {
        // In response to the slash command, toggle the display status of our main ui
    }
}
