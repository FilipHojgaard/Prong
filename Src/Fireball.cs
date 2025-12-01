using Godot;
using Prong.Shared;
using Prong.Src.Blocks;

namespace Prong.Src;

public partial class Fireball : RigidBody2D
{
    public int Speed { get; set; }
    public bool HitBall { get; set; } = false;
    public Prong Owner { get; set; }
    public DiagonalTypeEnum Diagonal { get; set; }

    private AudioStreamPlayer _hitSfx;

    // TODO: Implement diagonal here, and an Initialize method to set it. then in LinearVelocity under _Process, we can set the 0 to be the diaganal value or 0 dynamically for level 3 attack. 

    public override void _Ready()
    {
        _hitSfx = GetNode<AudioStreamPlayer>("HitSfx");

        GravityScale = 0;
        //Mass = 0f; // TODO: This does not work. It seems to work when I don't set the Mass at all. Check out if we reallyl need it. 
        LockRotation = true;

        ContactMonitor = true;
        MaxContactsReported = 20;

        CollisionLayer = 3;
        CollisionMask = 2 | 3 | 4;

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
    }

    public void Initialize(Prong owner, int speed, DiagonalTypeEnum diagonal = DiagonalTypeEnum.Straight)
    {
        Owner = owner;
        Speed = speed;
        Diagonal = diagonal;
    }

    public void FireLeft()
    {
        Rotation = Mathf.Pi;
        Speed *= -1;
    }

    public void Deconstruct()
    {
        QueueFree();
    }

    private void OnBodyEntered(Rid bodyRid, Node body, long bodyShapeIndex, long localShapeIndex)
    {
        if (body is Block block)
        {
            block.TakeHit();
            QueueFree();
        }
        if (body is Prong player)
        {
            _hitSfx.Reparent(GetTree().Root);
            _hitSfx.Finished += () => _hitSfx.QueueFree();
            _hitSfx.Play();

            QueueFree();
        }
        if (body is Fireball otherFireball)
        {
            if (GetInstanceId() < otherFireball.GetInstanceId())
            {
                _hitSfx.Reparent(GetTree().Root);
                _hitSfx.Finished += () => _hitSfx.QueueFree();
                _hitSfx.Play();
            }
            QueueFree();
        }
    }
}
