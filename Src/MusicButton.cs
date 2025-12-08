using Godot;

namespace Prong.Src;

public partial class MusicButton : TextureButton
{
    [Export]
    Texture2D MusicOff;
    [Export]
    Texture2D MusicOn;


    public override void _Ready()
    {
        TextureNormal = GameManager.MusicOn ? MusicOn : MusicOff;

        Pressed += OnPressed;
    }

    private void OnPressed()
    {
        GameManager.MusicOn = !GameManager.MusicOn;
        TextureNormal = GameManager.MusicOn ? MusicOn : MusicOff;
    }
}
