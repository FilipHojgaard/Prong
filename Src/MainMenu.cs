using Godot;

namespace Prong.Src;

public partial class MainMenu : Control
{
    public override void _Ready()
    {
        var playButton = GetNode<Button>("VBoxContainer/Play");
        playButton.Pressed += PlayGame;

        var exitButton = GetNode<Button>("VBoxContainer/Exit");
        exitButton.Pressed += ExitGame;
    }


    private void PlayGame()
    {
        GameManager.Instance.StartGame();
    }

    private void ExitGame()
    {
        GetTree().Quit();
    }
}
