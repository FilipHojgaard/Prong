using Godot;

namespace Prong.Src;

public partial class MainMenu : Control
{

    Button PlayButton;
    Button AboutButton;
    Button ExitButton;
    AudioStreamPlayer HoverSfx;
    AudioStreamPlayer ClickSfx;

    public override void _Ready()
    {
        PlayButton = GetNode<Button>("VBoxContainer/Play");
        PlayButton.Pressed += PlayGame;
        PlayButton.MouseEntered += OnButtonHovered;

        AboutButton = GetNode<Button>("VBoxContainer/About");
        AboutButton.MouseEntered += OnButtonHovered;

        ExitButton = GetNode<Button>("VBoxContainer/Exit");
        ExitButton.Pressed += ExitGame;
        ExitButton.MouseEntered += OnButtonHovered;

        HoverSfx = GetNode<AudioStreamPlayer>("HoverSfx");
        ClickSfx = GetNode<AudioStreamPlayer>("ClickSfx");
    }

    private void OnButtonHovered()
    {
        HoverSfx.Play();
    }

    private void PlayGame()
    {
        ClickSfx.Play();
        GameManager.Instance.StartGame();
    }

    
    private void ExitGame()
    {
        GetTree().Quit();
    }
}
