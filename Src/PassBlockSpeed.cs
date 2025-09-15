using Godot;

namespace Prong.Src;

public partial class PassBlockSpeed : Area2D
{
    public override void _Ready()
    {
        BodyEntered += BallHit;
    }
    
    private void BallHit(Node2D node)
    {
        if (node is Ball ball)
        {
            QueueFree();
            ball.HandlePassBlockSpeed();
        }
    }
}
