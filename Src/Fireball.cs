using Godot;

namespace Prong.Src;

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
    }

    public void FireLeft()
    {
        Rotation = Mathf.Pi;
        Speed *= -1;
    }

    private void OnBodyEntered(Rid bodyRid, Node body, long bodyShapeIndex, long localShapeIndex)
    {
        if (body is Block block)
        {
            block.QueueFree();
            GameManager.ShowEasterEggCounter = false;
        }
        if (body is Prong player)
        {
            QueueFree();
            GameManager.ShowEasterEggCounter = false;
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

