using Godot;

namespace Prong.Src;

public partial class Ball : RigidBody2D
{
    [Export]
    public float Speed { get; set; } = 550f;
    public bool SpawnInCenter { get; set; } = true;
    public Prong LastProngHit { get; set; } = null;

    public override void _Ready()
    {
        GravityScale = 0;

        ContactMonitor = true;
        MaxContactsReported = 20;

        BodyShapeEntered += OnBodyEntered;

        SetupDefaultBallMaterial();
        if (SpawnInCenter)
        {
            StartRandomDirection();
        }
    }

    public override void _Process(double delta)
    {
        CheckForGoal();
    }

    public override void _PhysicsProcess(double delta)
    {
        LinearVelocity = LinearVelocity.Normalized() * Speed;
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

    private void CheckForGoal()
    {
        if (Position.X <= GameManager.LeftBoundaryPosition)
        {
            GameManager.Player1Score++;
            GameManager.PrintScore();
            QueueFree();
            GameManager.DecreaseBallCount();
        }

        if (Position.X >= GameManager.RightBoundaryPosition)
        {
            GameManager.Player2Score++;
            GameManager.PrintScore();
            QueueFree();
            GameManager.DecreaseBallCount();
        }
    }

    private void OnBodyEntered(Rid bodyRid, Node body, long bodyShapeIndex, long localShapeIndex)
    {
        if (body is Prong paddle)
        {
            HandlePaddleCollision(paddle);
            if (Speed <= 900)
            {
                Speed += 40;
            }
            LastProngHit = paddle;
        }
        if (body is Block block)
        {
            HandleBlockCollision(block);
        }
    }

    private void HandleBlockCollision(Block block)
    {
        block.QueueFree();
    }

    public void HandlePassBlockSpeed()
    {
        if (LastProngHit != null)
        {
            GD.Print("ball hit passblockspeed");
            LastProngHit.IncreaseSpeed();
        }
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
