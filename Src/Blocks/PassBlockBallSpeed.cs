using Godot;
using Prong.Shared;

namespace Prong.Src.Blocks;

public partial class PassBlockBallSpeed : Area2D
{

    public override void _Ready()
    {
        BodyEntered += BallHit;
    }

    private void BallHit(Node2D node)
    {
        if (node is Ball ball)
        {
            //_hitSfx.Reparent(GetTree().Root);
            //_hitSfx.Finished += () => _hitSfx.QueueFree();
            //_hitSfx.Play();

            ball.BoostSpeed();
            QueueFree();
        }
    }
}
