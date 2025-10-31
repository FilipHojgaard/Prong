using Godot;

namespace Prong.Src.Blocks;

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
            QueueFree();
            GameManager.BallCount++;
            float movementAngle = ball.LinearVelocity.Angle();
            GameManager.SpawnBallAtPosition(ball.Position, movementAngle);
        }
    }
}
