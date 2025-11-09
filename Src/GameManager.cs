using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Godot;
using Prong.Shared;

namespace Prong.Src;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }
    public static Vector2 UpperBoundaryPosition { get; set; }
    public static Vector2 LowerBoundaryPosition { get; set; }
    public static float LeftBoundaryPosition { get; set; }
    public static float RightBoundaryPosition { get; set; }
    public static int RightPlayerScore { get; set; } = 0;
    public static int LeftPlayerScore { get; set; } = 0;
    public static int BallCount { get; set; } = 0;
    public static EasterEggStatusEnum EasterEggStatus { get; set; } = EasterEggStatusEnum.Inactive;
    public static bool ShowEasterEgg { get; set; } = false;
    private Dictionary<int, string> _maps { get; set; }

    private Queue<int> _mapHistory { get; set; }

    public bool Pause { get; set; } = false;
    public override void _Ready()
    {
        Instance = this;
        ProcessMode = ProcessModeEnum.Always; // To be able to unpuase again. 

        SetHorizontalBorders();
        SetVerticalBorders();

        _maps = new Dictionary<int, string>()
        {
            {0, "map_original"},
            {1, "map_fastAndFurious"},
            {2, "map_bases"},
            {3, "map_growth"},
            {4, "map_scarce" },
        };
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustReleased("Test"))
        {
            BallCount++;
            SpawnBall();
        }
        CheckForButtonPresses();
        CheckBallCount();
    }

    public void SetHorizontalBorders()
    {
        var borderScene = GD.Load<PackedScene>("res://Scenes/horizontal_border.tscn");
        var upperBorder = borderScene.Instantiate<StaticBody2D>();
        var lowerBorder = borderScene.Instantiate<StaticBody2D>();

        var camera = GetTree().CurrentScene.GetNodeOrNull<Camera2D>("Camera2D");
        var viewportSize = GetViewport().GetVisibleRect().Size;
        var cameraPos = camera.GlobalPosition;
        var halfViewportHeight = viewportSize.Y / 2;

        UpperBoundaryPosition = new Vector2(cameraPos.X, cameraPos.Y - halfViewportHeight + 10);
        LowerBoundaryPosition = new Vector2(cameraPos.X, cameraPos.Y + halfViewportHeight - 10);

        upperBorder.Position = UpperBoundaryPosition;
        upperBorder.Rotation = Mathf.Pi;
        lowerBorder.Position = LowerBoundaryPosition;

        GetTree().CurrentScene.AddChild(upperBorder);
        GetTree().CurrentScene.AddChild(lowerBorder);
    }
    
    public void SetVerticalBorders()
    {
        var camera = GetTree().CurrentScene.GetNodeOrNull<Camera2D>("Camera2D");
        var viewPortSize = GetViewport().GetVisibleRect().Size;
        var cameraPos = camera.GlobalPosition;
        var halfViewPortLength = viewPortSize.X / 2;

        LeftBoundaryPosition = cameraPos.X - halfViewPortLength;
        RightBoundaryPosition = cameraPos.X + halfViewPortLength;
    }

    public static void SpawnBallAtPosition(Vector2 position, float rotation)
    {
        var ballScene = GD.Load<PackedScene>("res://Scenes/ball.tscn");
        var ball = ballScene.Instantiate<Ball>();

        ball.SpawnInCenter = false;
        ball.StartAtPosition(position, rotation);

        Instance.GetTree().CurrentScene.AddChild(ball);
    }

    public static void SpawnBallAtCenter()
    {
        BallCount++;
        Instance.SpawnBall();
    }

    private async void CheckForButtonPresses()
    {
        if (Input.IsActionJustPressed("Reset"))
        {
            BallCount = default;
            LeftPlayerScore = default;
            RightPlayerScore = default;
            Pause = false;
            GetTree().Paused = Pause;
            GetTree().ReloadCurrentScene();

            // Waiting one frame for it to reset scene, then calling SetHorizontalBorders on the new scene. 
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            SetHorizontalBorders();
        }
        if (Input.IsActionJustPressed("Pause"))
        {
            Pause = !Pause;
            GetTree().Paused = Pause;
        }
        if (Input.IsActionJustPressed("Next"))
        {
            PickRandomMap();
        }
    }

    private async void PickRandomMap()
    {
        // TODO: Implement such that the same map can't be picked again.  

        BallCount = default;
        LeftPlayerScore = default;
        RightPlayerScore = default;
        Pause = false;
        GetTree().Paused = Pause;
        var mapName = _maps[GD.RandRange(0, _maps.Count)];
        GetTree().ChangeSceneToFile($"res://Scenes/Maps/{mapName}.tscn");

        // Waiting one frame for it to reset scene, then calling SetHorizontalBorders on the new scene. 
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        SetHorizontalBorders();
    }

    public void CheckForWinner()
    {
        if (LeftPlayerScore >= 10 || RightPlayerScore >= 10)
        {
            PickRandomMap();
        }
    }

    private void SpawnBall()
    {
        // Fetching the ball scene and instantiating a ball using it. 
        var ballScene = GD.Load<PackedScene>("res://Scenes/ball.tscn");
        var ball = ballScene.Instantiate<Ball>();

        // Because I have a camera on the game scene, I use that to determine the center of the screen for sparning the balls. 
        var camera = GetTree().CurrentScene.GetNodeOrNull<Camera2D>("Camera2D");
        ball.Position = camera.GlobalPosition;

        // Placing the ball on the current scene.
        GetTree().CurrentScene.AddChild(ball);
        
        // Run ball code when spawned in middle
        ball.SpawnedInMiddle();
    }

    private async void CheckBallCount()
    {
        if (BallCount <= 0)
        {
            BallCount++;
            await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
            SpawnBall();
        }
    }

    public static void DecreaseBallCount()
    {
        BallCount--;
    }

    public static void PrintScore()
    {
        GD.Print($"{LeftPlayerScore} - {RightPlayerScore}");
    }

    public static async void HandleEasterEgg()
    {
        if (EasterEggStatus == EasterEggStatusEnum.ReadyForEasterEgg)
        {
            ShowEasterEgg = true;
            await Instance.ToSignal(Instance.GetTree().CreateTimer(3.0), SceneTreeTimer.SignalName.Timeout);
            ShowEasterEgg = false;
            EasterEggStatus = EasterEggStatusEnum.Inactive;
        }
        else 
        { 
            EasterEggStatus = EasterEggStatusEnum.ReadyForEasterEgg;
        }

    }

}
