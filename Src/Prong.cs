using Godot;
using Prong.Src;

namespace Prong;

public partial class Prong : StaticBody2D
{
    [Export]
    public float speed = 300.0f;

    [Signal]
    public delegate void HighPositionEventHandler();

    public override void _Ready()
    {
        SetupGodotProperties();

        HighPosition += () => GD.Print("Listened to event from same place as definition");
    }

    private void SetupGodotProperties()
    {
        // Legacy from when Prong class was a RigidBody2D. 
        //GravityScale = 0;
        //LockRotation = true;

        //FreezeMode = RigidBody2D.FreezeModeEnum.Kinematic;
    }

    public override void _PhysicsProcess(double delta)
    {
        HandlePlayerInput(delta);

        SignalTesting();
    }

    private void SignalTesting()
    {
        if (Position.Y < -600)
        {
            EmitSignal(SignalName.HighPosition);
        }
    }

    private void HandlePlayerInput(double delta)
    {
        Vector2 velocity = Vector2.Zero;

        if (Input.IsActionPressed("Up"))
        {
            velocity.Y -= speed;
            GameData.incrementUp();
        }
        if (Input.IsActionPressed("Down"))
        {
            velocity.Y += speed;
        }
        if (Input.IsActionJustReleased("Space"))
        {
            GD.Print($"Pressed up {GameManager.data} time");
        }

        Position += velocity * (float)delta;
    }
}
