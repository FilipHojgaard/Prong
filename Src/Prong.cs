using Godot;
using Prong.Src;

namespace Prong;

public partial class Prong : StaticBody2D
{
    [Export]
    public float speed = 300.0f;

    [Export]
    public bool player2 { get; set; } = false;

    public override void _Ready()
    {
        SetupProperties();
    }

    private void SetupProperties()
    {
        // Legacy from when Prong class was a RigidBody2D. 
        //GravityScale = 0;
        //LockRotation = true;

        //FreezeMode = RigidBody2D.FreezeModeEnum.Kinematic;
    }

    public override void _PhysicsProcess(double delta)
    {
        HandlePlayerInput(delta);
    }

    private void HandlePlayerInput(double delta)
    {
        Vector2 velocity = Vector2.Zero;

        if (player2)
        {
            if (Input.IsActionPressed("Player2Up") && Position.Y > GameManager.UpperBoundaryPosition.Y)
            {
                velocity.Y -= speed;
            }
            if (Input.IsActionPressed("Player2Down") && Position.Y < GameManager.LowerBoundaryPosition.Y)
            {
                velocity.Y += speed;
            }
        }
        else
        {
            if (Input.IsActionPressed("Up") && Position.Y > GameManager.UpperBoundaryPosition.Y)
            {
                velocity.Y -= speed;
            }
            if (Input.IsActionPressed("Down") && Position.Y < GameManager.LowerBoundaryPosition.Y)
            {
                velocity.Y += speed;
            }
        }

        Position += velocity * (float)delta;
    }
}
