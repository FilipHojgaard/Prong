using Godot;
using Prong.Shared;

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
            var eventBus = GetNode<Eventbus>(ProngConstants.EventHubPath); // TODO:  Can I avoid getting a static reference somehow? 
            eventBus.EmitSignal(Eventbus.SignalName.SpeedLevelUp, ball.LastProngHit.player2);
        }
    }
}
