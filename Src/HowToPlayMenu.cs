using Godot;
namespace Prong.Src;

public partial class HowToPlayMenu : Control
{
    Button backButton;
    AudioStreamPlayer hoversfx;

    public override void _Ready()
    {
        hoversfx = GetNode<AudioStreamPlayer>("hoversfx");

        backButton = GetNode<Button>("back");
        backButton.Pressed += Back;
        backButton.MouseEntered += OnMouseHover;
        backButton.GrabFocus();

    }

    private void Back()
    {
        GetTree().ChangeSceneToFile($"res://Scenes/MainMenu.tscn");
    }

    private void OnMouseHover()
    {
        hoversfx.Play();
    }

}
