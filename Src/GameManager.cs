using System.Collections.Generic;
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
    public static int RightPlayerOverallScore { get; set; } = 0;
    public static int LeftPlayerOverallScore { get; set; } = 0;
    public static int RightPlayerOverallScorePrevious { get; set; } = 0;
    public static int LeftPlayerOverallScorePrevious { get; set; } = 0;
    public static int BallCount { get; set; } = 0;
    public static EasterEggStatusEnum EasterEggStatus { get; set; } = EasterEggStatusEnum.Inactive;
    public static bool ShowEasterEgg { get; set; } = false;
    public static bool LockNewRoundWinner { get; set; } = false;

    public bool InMainMenu { get; set; } = true;

    private PauseMenu _pauseMenu;
    private PackedScene _pauseMenuScene;

    private ScoreOverview _scoreOverview;
    private PackedScene _scoreOverviewScene;

    private Dictionary<int, string> _maps { get; set; }

    private int? _mapHistory { get; set; } = null;

    public bool Pause { get; set; } = false;
    public override void _Ready()
    {
        GD.Print("IN READY");
        Instance = this;
        ProcessMode = ProcessModeEnum.Always; // To be able to unpuase again. 

        _pauseMenuScene = GD.Load<PackedScene>("res://Scenes/PauseMenu.tscn");
        _scoreOverviewScene = GD.Load<PackedScene>("res://Scenes/ScoreScene.tscn");

        if (!InMainMenu)
        {
            SetHorizontalBorders();
            SetVerticalBorders();
        }

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
        if (InMainMenu)
        {
            return;
        }
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

        GD.Print("in sethorizontalborders");

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

        GD.Print(UpperBoundaryPosition);
        GD.Print(LowerBoundaryPosition);
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
            TogglePause();
        }
        if (Input.IsActionJustPressed("Next"))
        {
            PickRandomMap();
        }
    }

    public void TogglePause()
    {
        Pause = !Pause;
        GetTree().Paused = Pause;

        if (Pause)
        {
            ShowPauseMenu();
        }
        else
        {
            HidePauseMenu();
        }
    }

    private void ShowPauseMenu()
    {
        GD.Print("showing pause menu");
        _pauseMenu = _pauseMenuScene.Instantiate<PauseMenu>();
        _pauseMenu.ProcessMode = ProcessModeEnum.Always;
        //GetTree().CurrentScene.AddChild(_pauseMenu);
        GetTree().Root.AddChild(_pauseMenu);
    }

    private void HidePauseMenu()
    {
        if (_pauseMenu != null)
        {
            _pauseMenu.QueueFree();
        }
    }

    public void GoToMainMenu()
    {
        InMainMenu = true;
        BallCount = default;
        LeftPlayerScore = default;
        RightPlayerScore = default;
        RightPlayerOverallScore = default;
        LeftPlayerOverallScore = default;
        RightPlayerOverallScorePrevious = default;
        LeftPlayerOverallScorePrevious = default;
        GetTree().ChangeSceneToFile($"res://Scenes/MainMenu.tscn");
    }

    public async void StartGame()
    {
        // TODO: Implement such that the same map can't be picked again.  
        InMainMenu = false;
        BallCount = default;
        LeftPlayerScore = default;
        RightPlayerScore = default;
        Pause = false;
        GetTree().Paused = Pause;
        int? newMap = null;
        while (newMap == _mapHistory || newMap is null)
        {
            newMap = GD.RandRange(0, _maps.Count - 1);
        }
        _mapHistory = (int)newMap;
        var mapName = _maps[(int)newMap];
        GetTree().ChangeSceneToFile($"res://Scenes/Maps/{mapName}.tscn");

        // Waiting one frame for it to reset scene, then calling SetHorizontalBorders on the new scene. 
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); // Wait for new treescene
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); // Wait for nodes to load
        Instance.SetHorizontalBorders();
        Instance.SetVerticalBorders();
    }

    private async void PickRandomMap()
    {
        LockNewRoundWinner = false;
        // TODO: Implement such that the same map can't be picked again.  
        BallCount = default;
        LeftPlayerScore = default;
        RightPlayerScore = default;
        Pause = false;
        GetTree().Paused = Pause;
        int? newMap = null;
        while (newMap == _mapHistory || newMap is null)
        {
            newMap = GD.RandRange(0, _maps.Count - 1);
        }
        _mapHistory = (int)newMap;
        var mapName = _maps[(int)newMap];
        GetTree().ChangeSceneToFile($"res://Scenes/Maps/{mapName}.tscn");

        // Wait for the new scehe tree to change
        //await ToSignal(GetTree(), SceneTree.SignalName.TreeChanged);
        // Waiting one frame for it to reset scene, then calling SetHorizontalBorders on the new scene. 
        //await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        Instance.SetHorizontalBorders();
    }

    public void CheckForWinner()
    {
        if (LeftPlayerScore == 8)
        {
            HandleRoundWin(PlayerEnum.LeftPlayer);
        }
        if (RightPlayerScore == 8)
        {
            HandleRoundWin(PlayerEnum.RightPlayer);
        }
    }

    private async void HandleRoundWin(PlayerEnum player)
    {
        if (LockNewRoundWinner)
        {
            return;
        }
        // Lock round win
        LockNewRoundWinner = true;
        RemoveAllBalls();
        // Increment overall score
        if (player == PlayerEnum.LeftPlayer)
        {
            LeftPlayerOverallScore++;
        }
        else if (player == PlayerEnum.RightPlayer)
        {
            RightPlayerOverallScore++;
        }
    
        // Show score UI scene
        GD.Print("Showing ui score");
        _scoreOverview = _scoreOverviewScene.Instantiate<ScoreOverview>();
        GetTree().Root.AddChild( _scoreOverview );
        // Wait two frame for scoreOverview UI to have called _Ready()
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        _scoreOverview.Initialize(player);

        await ToSignal(GetTree().CreateTimer(1.5), SceneTreeTimer.SignalName.Timeout);
        _scoreOverview.AnimateScores();
        await ToSignal(GetTree().CreateTimer(2.5), SceneTreeTimer.SignalName.Timeout);
        //_scoreOverview.AnimationPlayer.AnimationFinished += (anyAnimationName) => 
        _scoreOverview.QueueFree();

        // Update previous overall score
        LeftPlayerOverallScorePrevious = LeftPlayerOverallScore;
        RightPlayerOverallScorePrevious = RightPlayerOverallScore;
        
        // Pick new map
        PickRandomMap();
    }

    private void RemoveAllBalls()
    {
        var balls = GetTree().GetNodesInGroup("Balls");
        foreach (Node ball in balls)
        {
            ball.QueueFree();
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
