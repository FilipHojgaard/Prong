using Godot;
using Prong.Shared;

namespace Prong.Src;

public partial class ScoreLabel : Label
{
    [Export]
    public PlayerEnum Player { get; set; } = PlayerEnum.Undefined;

    public override void _Ready()
    {
        Modulate = new Color(1, 1, 1, 0.1f);
    }

    public override void _Process(double delta)
    {
        if (Player == PlayerEnum.LeftPlayer)
        {
            Text = GameManager.ShowEasterEgg ? "10" : GameManager.RightPlayerScore.ToString();
        }
        else
        {
            Text = GameManager.ShowEasterEgg ? "28" : GameManager.LeftPlayerScore.ToString();
        }
    }
}
