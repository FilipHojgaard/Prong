using Godot;

namespace Prong.Src;

public partial class Ball : RigidBody2D
{
    [Export]
    public float Speed { get; set; } = 300f;

    public override void _Ready()
    {
        GravityScale = 0;

        StartRandomDirection();
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

}
