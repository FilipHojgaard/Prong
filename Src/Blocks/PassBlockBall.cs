using Godot;
using Prong.Shared;

namespace Prong.Src.Blocks;

public partial class PassBlockBall : Area2D
{
    private AudioStreamPlayer _passBlockBallHitSfx;
    public override void _Ready()
    {
        _passBlockBallHitSfx = GetNode<AudioStreamPlayer>("PassBlockBallHitSfx");

        CollisionLayer = 4;
        CollisionMask = 3;

        BodyEntered += BallHit;
    }

    private void BallHit(Node2D node)
    {
        if (node is Ball ball)
        {
            _passBlockBallHitSfx.Reparent(GetTree().Root);
            _passBlockBallHitSfx.Finished += () => _passBlockBallHitSfx.QueueFree();
            _passBlockBallHitSfx.Play();
                        
            float movementAngle = ball.LinearVelocity.Angle();
            var eventBus = GetNode<Eventbus>(ProngConstants.EventHubPath);
            eventBus.EmitSignal(Eventbus.SignalName.PassBlockBall, ball.Position, movementAngle);

            QueueFree();
        }
    }
}
