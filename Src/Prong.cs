using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
//using Godot.Collections;
using Prong.Shared;
using Prong.Src;
using Prong.Src.Blocks;

namespace Prong;

public partial class Prong : StaticBody2D
{
    [Export]
    public PlayerEnum Player { get; set; } = PlayerEnum.Undefined;

    public bool FireballReady { get; set; } = true;

    public bool ShieldUp { get; set; } = true;

    public Sprite2D indicator_1 { get; set; }

    public Sprite2D indicator_2 { get; set; }

    public int SpeedLevel { get; set; } = 1;

    public int AttackLevel { get; set; } = 1;

    public Dictionary<int, float> SpeedLevelDict = new Dictionary<int, float>()
    {
        { 1, 300f },
        { 2, 380f },
        { 3, 520f },
    };

    public Dictionary<int, float> AttackCooldownDict = new Dictionary<int, float>()
    {
        { 1, 10f },
        { 2, 8f },
        { 3, 7f },
    };

    public Dictionary<int, int> AttackSpeed = new Dictionary<int, int>()
    {
        { 1,  900 },
        { 2, 1100 },
        { 3, 1300 },
    };

    private Dictionary<int, Action> AttackActionDict;

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
        AttackActionDict = new Dictionary<int, Action>()
        {
            { 1, FireFireballLevel1 },
            { 2, FireFireballLevel2 },
            { 3, FireFireballLevel3 }
        };
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
        if (!FireballReady)
        {
            return;
        }

        AttackActionDict[AttackLevel]();

        FireballReady = false;
        SpriteUpdate();
        await ToSignal(GetTree().CreateTimer(AttackCooldownDict[AttackLevel]), SceneTreeTimer.SignalName.Timeout);
        FireballReady = true;
        SpriteUpdate();
    }

    
    private void FireFireballLevel1()
    {
        var fireBallScene = GD.Load<PackedScene>("res://Scenes/fireball.tscn");
        var fireball = fireBallScene.Instantiate<Fireball>();
        fireball.Initialize(AttackSpeed[AttackLevel]);

        if (Player == PlayerEnum.RightPlayer)
        {
            fireball.FireLeft();
        }

        var offset = (Player == PlayerEnum.RightPlayer) ? -20 : 20;
        fireball.Position = new Vector2(Position.X + offset, Position.Y);

        GetTree().CurrentScene.AddChild(fireball);
    }

    void FireFireballLevel2()
    {
        var fireBallScene = GD.Load<PackedScene>("res://Scenes/fireball.tscn");
        var fireballUpper = fireBallScene.Instantiate<Fireball>();
        var fireballLower = fireBallScene.Instantiate<Fireball>();

        fireballUpper.Initialize(AttackSpeed[AttackLevel]);
        fireballLower.Initialize(AttackSpeed[AttackLevel]);

        if (Player == PlayerEnum.RightPlayer)
        {
            fireballUpper.FireLeft();
            fireballLower.FireLeft();
        }

        var offset = (Player == PlayerEnum.RightPlayer) ? -20 : 20;
        fireballUpper.Position = new Vector2(Position.X + offset, Position.Y + 15);
        fireballLower.Position = new Vector2(Position.X + offset, Position.Y - 15);

        GetTree().CurrentScene.AddChild(fireballUpper);
        GetTree().CurrentScene.AddChild(fireballLower);
    }

    async void FireFireballLevel3()
    {
        var fireBallScene = GD.Load<PackedScene>("res://Scenes/fireball.tscn");
        var fireballUpper = fireBallScene.Instantiate<Fireball>();
        var fireballLower = fireBallScene.Instantiate<Fireball>();
        var fireballUpperDiagonally = fireBallScene.Instantiate<Fireball>();
        var fireballLowerDiagonally = fireBallScene.Instantiate<Fireball>();

        fireballUpper.Initialize(AttackSpeed[AttackLevel]);
        fireballLower.Initialize(AttackSpeed[AttackLevel]);
        fireballUpperDiagonally.Initialize(AttackSpeed[AttackLevel], diagonal: DiagonalTypeEnum.Upwards);
        fireballLowerDiagonally.Initialize(AttackSpeed[AttackLevel], diagonal: DiagonalTypeEnum.Downwards);

        if (Player == PlayerEnum.RightPlayer)
        {
            fireballUpper.FireLeft();
            fireballLower.FireLeft();
            fireballUpperDiagonally.FireLeft();
            fireballLowerDiagonally.FireLeft();
        }

        var offset = (Player == PlayerEnum.RightPlayer) ? -20 : 20;
        fireballUpper.Position = new Vector2(Position.X + offset, Position.Y + 15);
        fireballLower.Position = new Vector2(Position.X + offset, Position.Y - 15);
        fireballUpperDiagonally.Position = new Vector2(Position.X + offset, Position.Y + 40);
        fireballLowerDiagonally.Position = new Vector2(Position.X + offset, Position.Y - 40);

        GetTree().CurrentScene.AddChild(fireballUpper);
        GetTree().CurrentScene.AddChild(fireballLower);
        await ToSignal(GetTree().CreateTimer(0.02f), SceneTreeTimer.SignalName.Timeout);
        GetTree().CurrentScene.AddChild(fireballUpperDiagonally);
        GetTree().CurrentScene.AddChild(fireballLowerDiagonally);
    }

    private void SpriteUpdate()
    {
        Sprite2D sprite = GetNode<Sprite2D>("Sprite2D");
        Texture2D newTexture = null;
        
        if (FireballReady && ShieldUp && Player == PlayerEnum.LeftPlayer)
        {
            newTexture = GD.Load<Texture2D>("res://Assets/Sprites/paddle_red_blue.png");
        }
        else if (FireballReady && ShieldUp && Player == PlayerEnum.RightPlayer)
        {
            newTexture = GD.Load<Texture2D>("res://Assets/Sprites/paddle_blue_red.png");
        }
        else if (FireballReady)
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
        if (AttackLevel < 3)
        {
            AttackLevel++;
        }
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
