using Godot;
using Prong.Shared;

namespace Prong.Src.Blocks;

public partial class Fireball : RigidBody2D
{
    public int Speed { get; set; }
    public bool HitBall { get; set; } = false;
    public DiagonalTypeEnum Diagonal { get; set; }

    // TODO: Implement diagonal here, and an Initialize method to set it. then in LinearVelocity under _Process, we can set the 0 to be the diaganal value or 0 dynamically for level 3 attack. 

    public override void _Ready()
    {
        GravityScale = 0;
        Mass = 0;
        LockRotation = true;

        ContactMonitor = true;
        MaxContactsReported = 20;

        BodyShapeEntered += OnBodyEntered;
    }

    public override void _Process(double delta)
    {
        float yDirection;
        switch (Diagonal)
        {
            case DiagonalTypeEnum.Straight:
                yDirection = 0f;
                break;
            case DiagonalTypeEnum.Upwards:
                yDirection = -0.08f;
                break;
            case DiagonalTypeEnum.Downwards:
                yDirection = 0.08f;
                break;
            default:
                yDirection = 0f;
                break;
        }
        LinearVelocity = new Vector2(Speed, 0).Rotated(yDirection);
        CheckForLeftMap();
    }

    public void Initialize(int speed, DiagonalTypeEnum diagonal = DiagonalTypeEnum.Straight)
    {
        Speed = speed;
        Diagonal = diagonal;
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
            block.TakeHit();
            QueueFree();
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

