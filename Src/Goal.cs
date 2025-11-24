using Godot;
using Prong.Shared;

namespace Prong.Src;

public partial class Goal : Area2D
{
    public PlayerEnum Owner { get; set; } = PlayerEnum.Undefined;

    private AudioStreamPlayer _goal_sfx { get; set; }

    public override void _Ready()
    {
        BodyEntered += BallHit;

        _goal_sfx = GetNode<AudioStreamPlayer>("goal_sfx");
    }

    public void Initialize(PlayerEnum owner)
    {
        Owner = owner;

        if (Owner == PlayerEnum.RightPlayer)
        {
            Rotation = Mathf.Pi;
        }
    }

    private void BallHit(Node2D node)
    {
        if (node is Ball ball)
        {
            var playerScored = (Owner == PlayerEnum.LeftPlayer) ? PlayerEnum.RightPlayer : PlayerEnum.LeftPlayer;
            var eventBus = GetNode<Eventbus>(ProngConstants.EventHubPath);
            eventBus.EmitSignal(Eventbus.SignalName.Goal, (int)playerScored);

            _goal_sfx.Play();
            ball.QueueFree();
        }
    }

}
