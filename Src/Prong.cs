using System.Threading.Tasks;
using Godot;
using Prong.Src;

namespace Prong;

public partial class Prong : StaticBody2D
{
    [Export]
    public float speed = 300.0f;

    [Export]
    public bool player2 { get; set; } = false;

    public bool Ammo { get; set; } = true;

    public bool ShieldUp { get; set; } = true;

    public override void _Ready()
    {
        SetupProperties();
        SpriteUpdate();
    }

    public override void _EnterTree()
    {
        GetNode<Eventbus>("/root/Eventbus").SpeedLevelUp += EventIncreaseSpeed;
    }

    public override void _ExitTree()
    {
        GetNode<Eventbus>("/root/Eventbus").SpeedLevelUp -= EventIncreaseSpeed;
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
            if (Input.IsActionJustPressed("Player2Fire"))
            {
                FireFireball();
            }
            if (Input.IsActionJustPressed("Player2Defence"))
            {
                SetBlock();
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
            if (Input.IsActionJustPressed("Fire"))
            {
                FireFireball(fireLeft: true);
            }
            if (Input.IsActionJustPressed("Defence"))
            {
                SetBlock();
            }
        }

        Position += velocity * (float)delta;
    }

    public async Task FireFireball(bool fireLeft = false)
    {
        if (!Ammo)
        {
            return;
        }

        var fireBallScene = GD.Load<PackedScene>("res://Scenes/fireball.tscn");
        var fireball = fireBallScene.Instantiate<Fireball>();

        if (fireLeft) 
        { 
            fireball.FireLeft(); 
        }

        var offset = fireLeft ? -20 : 20;
        fireball.Position = new Vector2(Position.X + offset, Position.Y);

        GetTree().CurrentScene.AddChild(fireball);

        Ammo = false;
        SpriteUpdate();
        await ToSignal(GetTree().CreateTimer(10.0), SceneTreeTimer.SignalName.Timeout);
        Ammo = true;
        SpriteUpdate();
    }

    private void SpriteUpdate()
    {
        Sprite2D sprite = GetNode<Sprite2D>("Sprite2D");
        Texture2D newTexture = null;
        
        if (Ammo && ShieldUp && player2)
        {
            newTexture = GD.Load<Texture2D>("res://Assets/Sprites/paddle_red_blue.png");
        }
        else if (Ammo && ShieldUp && !player2)
        {
            newTexture = GD.Load<Texture2D>("res://Assets/Sprites/paddle_blue_red.png");
        }
        else if (Ammo)
        {
            newTexture = GD.Load<Texture2D>("res://Assets/Sprites/paddle_red.png");
        }
        else if (ShieldUp)
        {
            newTexture = GD.Load<Texture2D>("res://Assets/Sprites/paddle_blue.png");
        }
        else
        {
            newTexture = GD.Load<Texture2D>("res://Assets/Sprites/paddle.png");
        }

        if (newTexture is not null)
        {
            sprite.Texture = newTexture;
        }

    }

    public async Task SetBlock()
    {
        if (!ShieldUp)
        {
            return;
        }

        var blockScene = GD.Load<PackedScene>("res://Scenes/Block.tscn");
        var block = blockScene.Instantiate<Block>();

        var offset = player2 ? -20 : 20;

        block.Position = new Vector2(Position.X + offset, Position.Y);

        GetTree().CurrentScene.AddChild(block);

        ShieldUp = false;
        SpriteUpdate();
        await ToSignal(GetTree().CreateTimer(10.0), SceneTreeTimer.SignalName.Timeout);
        ShieldUp = true;
        SpriteUpdate();
    }

    public void IncreaseSpeed()
    {
        speed += 30;
        GD.Print(speed);
    }

    private void EventIncreaseSpeed(bool isPlayer2)
    {
        if (isPlayer2 != player2) // Only handle speed buff for the correct player
        {
            return;
        }
        GD.Print($"Player {player2} increasing speed from event");
    }
}
