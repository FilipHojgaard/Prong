using Godot;

namespace Prong.Src.Blocks;

public partial class PassBlockBall : Area2D
{
    private AudioStreamPlayer _passBlockBallHitSfx;
    public override void _Ready()
    {
        _passBlockBallHitSfx = GetNode<AudioStreamPlayer>("PassBlockBallHitSfx");

        BodyEntered += BallHit;
    }

    private void BallHit(Node2D node)
    {
        if (node is Ball ball)
        {
            _passBlockBallHitSfx.Reparent(GetTree().Root);
            _passBlockBallHitSfx.Finished += () => _passBlockBallHitSfx.QueueFree();
            _passBlockBallHitSfx.Play();

            QueueFree();
            GameManager.BallCount++;
            float movementAngle = ball.LinearVelocity.Angle();
            GameManager.SpawnBallAtPosition(ball.Position, movementAngle);
        }
    }
}
