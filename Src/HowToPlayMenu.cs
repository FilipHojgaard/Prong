using Godot;
using Newtonsoft.Json.Bson;

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

    }

    private void Back()
    {
        this.QueueFree();
    }

    private void OnMouseHover()
    {
        hoversfx.Play();
    }

}
