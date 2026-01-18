using Godot;

namespace Prong.Src;

public partial class PauseMenu : Control
{
    Button Continue;
    Button Menu;
    Button Exit;

    Label LeftScore;
    Label RightScore;

    AudioStreamPlayer Hoversfx;

    public override void _Ready()
    {
        // Score
        LeftScore = GetNode<Label>("CanvasLayer/LeftScore");
        RightScore = GetNode<Label>("CanvasLayer/RightScore");
        LeftScore.Text = GameManager.LeftPlayerScore.ToString();
        RightScore.Text = GameManager.RightPlayerScore.ToString();

        // State Machine
        GameManager.Instance.SetStateMachine(Shared.StateMachineEnum.InPauseMenu);

        // Buttons
        Continue = GetNode<Button>("CanvasLayer/VBoxContainer/Continue");
        Continue.MouseEntered += OnButtonHovered;
        Continue.Pressed += ContinueHandler;
        Continue.GrabFocus();

        Menu = GetNode<Button>("CanvasLayer/VBoxContainer/Menu");
        Menu.MouseEntered += OnButtonHovered;
        Menu.Pressed += MenuHandler;

        Exit = GetNode<Button>("CanvasLayer/VBoxContainer/Exit");
        Exit.MouseEntered += OnButtonHovered;
        Exit.Pressed += () => GetTree().Quit();

        // sfx
        Hoversfx = GetNode<AudioStreamPlayer>("HoverSfx");
    }

    private void ContinueHandler()
    {
        GD.Print("continuing");
        GameManager.Instance.SetStateMachine(Shared.StateMachineEnum.Playing);
        GameManager.Instance.TogglePause();
    }

    private void MenuHandler()
    {
        GD.Print("back to menu...");
        GameManager.Instance.GoToMainMenu();
        GameManager.Instance.TogglePause();
        QueueFree();
    }

    private void OnButtonHovered()
    {
        Hoversfx.Play();
    }
}
