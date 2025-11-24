using Godot;
using Prong.Shared;
using Prong.Src.Blocks;

namespace Prong.Src;

public partial class Ball : RigidBody2D
{
    [Export]
    public float Speed { get; set; } = 550f;

    public float MaxSpeed { get; set; } = 1500f;

    public bool SpawnInCenter { get; set; } = true;
    public Prong LastProngHit { get; set; } = null;
    public int Bounces { get; set; } = 0;

    public int VerticalBounces { get; set; } = 0;
    public int MaxVerticalBounces { get; } = 5;
    private int BounceThresholdForNewBall { get; } = 10;

    public int FireballHits { get; set; } = 0;

    private AudioStreamPlayer _prongHitSfx;

    private AudioStreamPlayer _goalSfx;

    private AudioStreamPlayer _otherHitSfx;

    private AudioStreamPlayer _spawnSfx;

    public override void _Ready()
    {
        GravityScale = 0;
        ContinuousCd = CcdMode.CastRay;

        ContactMonitor = true;
        MaxContactsReported = 20;

        AddToGroup("Balls");

        BodyShapeEntered += OnBodyEntered;

        _prongHitSfx = GetNode<AudioStreamPlayer>("ProngHitSfx");
        _goalSfx = GetNode<AudioStreamPlayer>("GoalSfx");
        _otherHitSfx = GetNode<AudioStreamPlayer>("OtherHitSfx");
        _spawnSfx = GetNode<AudioStreamPlayer>("SpawnSfx");
        
        SetupDefaultBallMaterial();
        if (SpawnInCenter)
        {
            StartRandomDirection();
        }

        SetStartSpeed();
    }

    public void SpawnedInMiddle()
    {
        _spawnSfx.Play();
    }

    public override void _Process(double delta)
    {
        if (GameManager.LockNewRoundWinner)
        {
            return;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        LinearVelocity = LinearVelocity.Normalized() * Speed;
    }

    public void SetStartSpeed()
    {
        if (GameManager.LeftPlayerScore + GameManager.RightPlayerScore >= 7)
        {
            Speed = 740;
        }
    }

    public void StartAtPosition(Vector2 position, float rotation)
    {
        Position = position;
        Rotation = rotation + Mathf.Pi;

        Vector2 velocity = new Vector2(Speed, 0).Rotated(Rotation);

        LinearVelocity = velocity;
    }

    private void StartRandomDirection()
    {
        // Angle between -45 and 45 degrees
        float randomAngle = (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);

        // Go left or right
        float direction = GD.Randf() > 0.5f ? 1.0f : -1.0f;

        Vector2 velocity = new Vector2(direction * Speed, 0).Rotated(randomAngle);

        LinearVelocity = velocity;
    }

    private void SetupDefaultBallMaterial()
    {
        var material = new PhysicsMaterial();
        material.Friction = 0f;
        material.Bounce = 1f;
        PhysicsMaterialOverride = material;
    }

    private void HandleBounceCount()
    {
        VerticalBounces = default;
        Bounces++;
        if (Bounces >= BounceThresholdForNewBall)
        {
            GameManager.SpawnBallAtCenter();
            Bounces = 0;
        }
    }

    public void BoostSpeed()
    {
        Speed += 400;
        if (Speed > MaxSpeed)
        {
            Speed = MaxSpeed;
        }
    }

    private void OnBodyEntered(Rid bodyRid, Node body, long bodyShapeIndex, long localShapeIndex)
    {
        if (body is Prong paddle)
        {
            _prongHitSfx.Play();
            HandlePaddleCollision(paddle);
            if (Speed <= MaxSpeed)
            {
                Speed += 60;
            }
            LastProngHit = paddle;
            HandleBounceCount();
        }
        if (body is Block block)
        {
            HandleBlockCollision(block);
            HandleBounceCount();
            LastProngHit = block.Owner;
        }
        if (body is Fireball fireball && !fireball.HitBall)
        {
            _otherHitSfx.Play();
            FireballHits++;
            fireball.HitBall = true;
            // TODO: Remove fireball hit easteregg trigger thorougly, after attack levels have multiple fireballs. 
            //if (FireballHits >= 2)
            //{
            //    HandleEasterEgg();
            //}
        }
        if (body is Ball otherBall)
        {
            if (GetInstanceId() < otherBall.GetInstanceId())
            {
                _otherHitSfx.Play();
            }
        }
        if (body is Border border)
        {
            _otherHitSfx.Play();
            VerticalBounces++;
            if (VerticalBounces >= MaxVerticalBounces)
            {
                QueueFree();
                GameManager.DecreaseBallCount();
                GameManager.SpawnBallAtCenter();
            }
        }
    }

    private void HandleBlockCollision(Block block)
    {
        block.TakeHit();
    }

    private async void HandleEasterEgg()
    {
        GD.Print("Handeling easter egg");
        GameManager.ShowEasterEgg = true;
        await ToSignal(GetTree().CreateTimer(3.0), SceneTreeTimer.SignalName.Timeout);
        GameManager.ShowEasterEgg = false;
    }

    private void HandlePaddleCollision(Prong paddle)
    {
        // Get collision point relative to paddle center
        float paddleHeight = 64.0f; // Adjust to your paddle height
        float ballY = GlobalPosition.Y;
        float paddleY = paddle.GlobalPosition.Y;

        //// Calculate hit position (-1 = top, 0 = center, +1 = bottom)
        float hitPosition = (ballY - paddleY) / (paddleHeight * 0.5f);

        //// Clamp to prevent extreme angles
        hitPosition = Mathf.Clamp(hitPosition, -1.0f, 1.0f);

        GD.Print(hitPosition);

        // Add vertical component based on hit position
        float maxAngle = Mathf.Pi / 3; // 60 degrees max
        float angle = hitPosition * maxAngle * 0.5f; // Scale down the angle
        angle = LinearVelocity.X > 0 ? angle : -angle;

        Vector2 newVelocity = new Vector2(LinearVelocity.X, 0).Rotated(angle);

        newVelocity = newVelocity.Normalized() * Speed;

        LinearVelocity = newVelocity;
    }

}
