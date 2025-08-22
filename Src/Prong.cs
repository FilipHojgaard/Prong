using Godot;
using Prong.Src;

namespace Prong;

public partial class Prong : RigidBody2D
{
    [Export]
    public float speed = 300.0f;

    [Signal]
    public delegate void HighPositionEventHandler();

    public override void _Ready()
    {
        GravityScale = 0;

        FreezeMode = RigidBody2D.FreezeModeEnum.Kinematic;

        HighPosition += () => GD.Print("Listened to event from same place as definition");
    }

    public override void _PhysicsProcess(double delta)
    {
        HandlePlayerInput();

        SignalTesting();
    }

    private void SignalTesting()
    {
        if (Position.Y < 50)
        {
            EmitSignal(SignalName.HighPosition);
        }
    }

    private void HandlePlayerInput()
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

        LinearVelocity = velocity;
    }
}
