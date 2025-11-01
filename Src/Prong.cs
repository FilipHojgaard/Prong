using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using Prong.Shared;
using Prong.Src;
using Prong.Src.Blocks;

namespace Prong;

public partial class Prong : StaticBody2D
{
    [Export]
    public PlayerEnum Player { get; set; } = PlayerEnum.Undefined;

    public bool Ammo { get; set; } = true;

    public bool ShieldUp { get; set; } = true;

    public Sprite2D indicator_1 { get; set; }

    public Sprite2D indicator_2 { get; set; }

    public int SpeedLevel { get; set; } = 1;

    public Dictionary<int, float> SpeedLevelDict = new Dictionary<int, float>()
    {
        { 1, 300f },
        { 2, 380f },
        { 3, 520f }
    };

    public override void _Ready()
    {
        SetupProperties();
        SpriteUpdate();

        indicator_1 = GetNode<Sprite2D>("speed_2");
        indicator_2 = GetNode<Sprite2D>("speed_3");
    }

    public override void _EnterTree()
    {
        GetNode<Eventbus>(ProngConstants.EventHubPath).SpeedLevelUp += EventIncreaseSpeed;
        GetNode<Eventbus>(ProngConstants.EventHubPath).AttackLevelUp += EventIncreaseAttack;
        GetNode<Eventbus>(ProngConstants.EventHubPath).DefenceLevelUp += EventIncreaseDefence;
    }

    public override void _ExitTree()
    {
        GetNode<Eventbus>(ProngConstants.EventHubPath).SpeedLevelUp -= EventIncreaseSpeed;
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

        if (Player == PlayerEnum.LeftPlayer)
        {
            if (Input.IsActionPressed("LeftUp") && Position.Y > GameManager.UpperBoundaryPosition.Y)
            {
                velocity.Y -= SpeedLevelDict[SpeedLevel];
            }
            if (Input.IsActionPressed("LeftDown") && Position.Y < GameManager.LowerBoundaryPosition.Y)
            {
                velocity.Y += SpeedLevelDict[SpeedLevel];
            }
            if (Input.IsActionJustPressed("LeftFire"))
            {
                FireFireball();
            }
            if (Input.IsActionJustPressed("LeftDefence"))
            {
                SetBlock();
            }
        }
        else
        {
            if (Input.IsActionPressed("RightUp") && Position.Y > GameManager.UpperBoundaryPosition.Y)
            {
                velocity.Y -= SpeedLevelDict[SpeedLevel];
            }
            if (Input.IsActionPressed("RightDown") && Position.Y < GameManager.LowerBoundaryPosition.Y)
            {
                velocity.Y += SpeedLevelDict[SpeedLevel];
            }
            if (Input.IsActionJustPressed("RightFire"))
            {
                FireFireball(fireLeft: true);
            }
            if (Input.IsActionJustPressed("RightDefence"))
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
        
        if (Ammo && ShieldUp && Player == PlayerEnum.LeftPlayer)
        {
            newTexture = GD.Load<Texture2D>("res://Assets/Sprites/paddle_red_blue.png");
        }
        else if (Ammo && ShieldUp && Player == PlayerEnum.RightPlayer)
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

        var offset = Player == PlayerEnum.LeftPlayer ? -20 : 20;

        block.Position = new Vector2(Position.X + offset, Position.Y);

        GetTree().CurrentScene.AddChild(block);

        ShieldUp = false;
        SpriteUpdate();
        await ToSignal(GetTree().CreateTimer(10.0), SceneTreeTimer.SignalName.Timeout);
        ShieldUp = true;
        SpriteUpdate();
    }

    private void EventIncreaseSpeed(int EventPlayer)
    {
        if (Player != (PlayerEnum)EventPlayer) // Only handle speed buff for the correct player
        {
            return;
        }

        if (SpeedLevel < 3)
        {
            SpeedLevel++;
        }

        if (SpeedLevel == 2)
        {
            indicator_1.Visible = true;
        }
        if (SpeedLevel == 3)
        {
            indicator_2.Visible = true;
        }
    }

    private void EventIncreaseAttack(int EventPlayer)
    {
        if (Player != (PlayerEnum)EventPlayer)
        {
            return;
        }
        GD.Print("Attack level increased");
    }

    private void EventIncreaseDefence(int EventPlayer)
    {
        if (Player != (PlayerEnum)EventPlayer)
        {
            return;
        }
        GD.Print("Defence level increased");
    }
}
