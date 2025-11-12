using Godot;

namespace Prong.Src;

public partial class PauseMenu : Control
{
    Button Continue;
    Button Menu;
    Button Exit;

    public override void _Ready()
    {
        GD.Print("in pause menu");
        Continue = GetNode<Button>("CanvasLayer/VBoxContainer/Continue");
        //Continue.ProcessMode = ProcessModeEnum.Always;
        Continue.Pressed += ContinueHandler;

        Menu = GetNode<Button>("CanvasLayer/VBoxContainer/Menu");
        //Menu.ProcessMode = ProcessModeEnum.Always;
        Menu.Pressed += MenuHandler;

        Exit = GetNode<Button>("CanvasLayer/VBoxContainer/Exit");
        //Exit.ProcessMode = ProcessModeEnum.Always;
        Exit.Pressed += () => GetTree().Quit();
    }

    private void ContinueHandler()
    {
        GD.Print("continuing");
        GameManager.Instance.TogglePause();
    }

    private void MenuHandler()
    {
        GD.Print("back to menu...");
    }
}
