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
            ball.HandlePassBlockSpeed(); // TODO: Remove this method call and rely only on the eventbus. 

            var eventBus = GetNode<Eventbus>("/root/Eventbus"); // TODO:  Can I avoid getting a static reference somehow? 
            eventBus.EmitSignal(Eventbus.SignalName.SpeedLevelUp, ball.LastProngHit.player2);
        }
    }
}
