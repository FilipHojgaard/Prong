using Godot;
using Prong.Shared;

namespace Prong.Src.Blocks;

public partial class Fireball : RigidBody2D
{
    public int Speed { get; set; } = 900;
    public bool HitBall { get; set; } = false;

    public override void _Ready()
    {
        GravityScale = 0;
        Mass = 1000;
        LockRotation = true;

        ContactMonitor = true;
        MaxContactsReported = 20;

        BodyShapeEntered += OnBodyEntered;
    }

    public override void _Process(double delta)
    {
        LinearVelocity = new Vector2(Speed, 0);
        CheckForLeftMap();
    }

    public void FireLeft()
    {
        Rotation = Mathf.Pi;
        Speed *= -1;
    }

    private void CheckForLeftMap()
    {
        if (Position.X > GameManager.RightBoundaryPosition + 50 || Position.X < GameManager.LeftBoundaryPosition - 50)
        {
            GameManager.EasterEggStatus = EasterEggStatusEnum.Inactive;
            QueueFree();
        }
    }

    private void OnBodyEntered(Rid bodyRid, Node body, long bodyShapeIndex, long localShapeIndex)
    {
        if (body is Block block)
        {
            block.QueueFree();
            GameManager.EasterEggStatus = EasterEggStatusEnum.Inactive;
        }
        if (body is Prong player)
        {
            QueueFree();
            GameManager.EasterEggStatus = EasterEggStatusEnum.Inactive;
        }
        if (body is Fireball otherFireball)
        {
            if (GetInstanceId() < otherFireball.GetInstanceId())
            {
                if (this.Position.Y <= GameManager.UpperBoundaryPosition.Y + 10)
                {
                    GameManager.HandleEasterEgg();
                }
            }
            QueueFree();
        }
    }
}

