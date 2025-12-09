using Godot;
using Prong.Shared;

namespace Prong.Src;

public partial class MainMenu : Control
{

    Button PlayButton;
    Button AboutButton;
    Button ExitButton;
    AudioStreamPlayer HoverSfx;
    AudioStreamPlayer ClickSfx;
    AudioStreamPlayer Music;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;

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
        Music = GetNode<AudioStreamPlayer>("Music");
        Music.Playing = GameManager.MusicOn;
        // TODO: Add MusicEvent and a handler for MainMenu and GenericMap that then sets the Music.Playing to GameManager.MusicOn whenever the event occurs. 

        GameManager.Instance.SetStateMachine(StateMachineEnum.InMainMenu);
    }

    public override void _EnterTree()
    {
        GetNode<Eventbus>(ProngConstants.EventHubPath).MusicSetting += HandleMusicSetting;
    }

    public override void _ExitTree()
    {
        GetNode<Eventbus>(ProngConstants.EventHubPath).MusicSetting -= HandleMusicSetting;
    }

    private void OnButtonHovered()
    {
        HoverSfx.Play();
    }

    private void PlayGame()
    {
        ClickSfx.Play();
        GameManager.Instance.SetStateMachine(StateMachineEnum.Playing);
        GameManager.Instance.StartGame();
    }

    private void ExitGame()
    {
        GetTree().Quit();
    }

    private void HandleMusicSetting()
    {
        Music.Playing = GameManager.MusicOn;
    }
}
