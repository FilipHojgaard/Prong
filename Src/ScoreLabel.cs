using Godot;

namespace Prong.Src;

public partial class ScoreLabel : Label
{
    [Export]
    public bool Player2 { get; set; } = false;

    public override void _Ready()
    {
        Modulate = new Color(1, 1, 1, 0.1f);
    }

    public override void _Process(double delta)
    {
        if (Player2)
        {
            Text = GameManager.ShowEasterEgg ? "10" : GameManager.Player1Score.ToString();
        }
        else
        {
            Text = GameManager.ShowEasterEgg ? "28" : GameManager.Player2Score.ToString();
        }
    }
}
