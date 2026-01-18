using Godot;
using Prong.Shared;

namespace Prong.Src;

public partial class MainMenu : Control
{

    Button PlayButton;
    Button HowToPlayButton;
    Button ExitButton;
    AudioStreamPlayer HoverSfx;
    AudioStreamPlayer ClickSfx;
    AudioStreamPlayer Music;

    private HowToPlayMenu _howToPlay;
    private PackedScene _howToPlayScene;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;

        _howToPlayScene = GD.Load<PackedScene>("res://Scenes/HowToPlay.tscn");

        PlayButton = GetNode<Button>("VBoxContainer/Play");
        PlayButton.Pressed += PlayGame;
        PlayButton.MouseEntered += OnButtonHovered;
        PlayButton.GrabFocus();

        HowToPlayButton = GetNode<Button>("VBoxContainer/HowToPlay");
        HowToPlayButton.Pressed += HowToPlay;
        HowToPlayButton.MouseEntered += OnButtonHovered;

        ExitButton = GetNode<Button>("VBoxContainer/Exit");
        ExitButton.Pressed += ExitGame;
        ExitButton.MouseEntered += OnButtonHovered;

        HoverSfx = GetNode<AudioStreamPlayer>("HoverSfx");
        ClickSfx = GetNode<AudioStreamPlayer>("ClickSfx");
        Music = GetNode<AudioStreamPlayer>("Music");
        Music.Playing = GameManager.MusicOn;

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

    private void HowToPlay()
    {
        GetTree().ChangeSceneToFile($"res://Scenes/HowToPlay.tscn");
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
