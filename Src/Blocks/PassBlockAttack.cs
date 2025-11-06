using Godot;
using Prong.Shared;

namespace Prong.Src.Blocks;

public partial class PassBlockAttack : Area2D
{
    private AudioStreamPlayer _hitSfx;
    public override void _Ready()
    {
        _hitSfx = GetNode<AudioStreamPlayer>("HitSfx");

        BodyEntered += BallHit;
    }

    private void BallHit(Node2D node)
    {
        if (node is Ball ball)
        {
            _hitSfx.Reparent(GetTree().Root);
            _hitSfx.Finished += () => _hitSfx.QueueFree();
            _hitSfx.Play();

            QueueFree();
            var eventBus = GetNode<Eventbus>(ProngConstants.EventHubPath); // TODO:  Can I avoid getting a static reference somehow? 
            eventBus.EmitSignal(Eventbus.SignalName.AttackLevelUp, (int)ball.LastProngHit.Player);
        }
    }
}
