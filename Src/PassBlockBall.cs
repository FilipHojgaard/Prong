using Godot;

namespace Prong.Src;

public partial class PassBlockBall : Area2D
{
    public override void _Ready()
    {
        BodyEntered += BallHit;
    }

    private void BallHit(Node2D node)
    {
        if (node is Ball ball)
        {
            GD.Print("ball hit");
            QueueFree();
        }
    }
}
