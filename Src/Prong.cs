using Godot;

namespace Prong;

public partial class Prong : RigidBody2D
{
    [Export]
    public float speed = 300.0f;

    public override void _Ready()
    {
        GravityScale = 0;

        FreezeMode = RigidBody2D.FreezeModeEnum.Kinematic;
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Vector2.Zero;

        if (Input.IsActionPressed("Up"))
        {
            velocity.Y -= speed;
        }
        if (Input.IsActionPressed("Down"))
        {
            velocity.Y += speed;
        }

        LinearVelocity = velocity;
    }
}
