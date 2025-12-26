using System.Collections.Generic;
using Godot;
using Prong.Shared;

namespace Prong.Src;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }
    public static Vector2 UpperBoundaryPosition { get; set; } = new Vector2(0, 0);
    public static Vector2 LowerBoundaryPosition { get; set; } = new Vector2(0, 0);
    public static float LeftGoalPosition { get; set; } = -1f;
    public static float RightGoalPosition { get; set; } = -1f;
    public static Vector2 MapCenter { get; set; } = new Vector2(0, 0);
    public static int RightPlayerScore { get; set; } = 0;
    public static int LeftPlayerScore { get; set; } = 0;
    public static int RightPlayerOverallScore { get; set; } = 0;
    public static int LeftPlayerOverallScore { get; set; } = 0;
    public static int RightPlayerOverallScorePrevious { get; set; } = 0;
    public static int LeftPlayerOverallScorePrevious { get; set; } = 0;
    public static bool LockNewRoundWinner { get; set; } = false;
    public static bool MusicOn { get; set; } = true;

    public StateMachineEnum StateMachine { get; set; } = StateMachineEnum.Undefined;
    public bool InMainMenu { get; set; } = true;

    private PauseMenu _pauseMenu;
    private PackedScene _pauseMenuScene;

    private ScoreOverview _scoreOverview;
    private PackedScene _scoreOverviewScene;

    private Dictionary<int, string> _maps { get; set; }

    protected BoundedQueue<int?> _mapHistory { get; set; } = new BoundedQueue<int?>(MaxSize: 6);

    public bool Pause { get; set; } = false;
    public override void _Ready()
    {
        GD.Print("IN READY");
        Instance = this;
        ProcessMode = ProcessModeEnum.Always; // To be able to unpuase again. 

        _pauseMenuScene = GD.Load<PackedScene>("res://Scenes/PauseMenu.tscn");
        _scoreOverviewScene = GD.Load<PackedScene>("res://Scenes/ScoreScene.tscn");

        _maps = new Dictionary<int, string>()
        {
            {0, "map_original"},
            {1, "map_fastAndFurious"},
            {2, "map_bases"},
            {3, "map_growth"},
            {4, "map_scarce" },
            {5, "map_love" },
            {6, "map_blocks" },
            {7, "map_tunnels" },
            {8, "map_strength" },
            {9, "map_x" },
            {10, "map_boxes" },
            {11, "map_slowlife" },
        };
    }

    public override void _Process(double delta)
    {
        if (InMainMenu)
        {
            return;
        }
        CheckForButtonPresses();
    }

    public override void _EnterTree()
    {
        GetNode<Eventbus>(ProngConstants.EventHubPath).Goal += GoalHandler;
    }

    public override void _ExitTree()
    {
        GetNode<Eventbus>(ProngConstants.EventHubPath).Goal -= GoalHandler;
    }

    public void CalculateBorderPositions()
    {
        // Only calculate border positions once
        if (UpperBoundaryPosition != new Vector2(0,0) && LowerBoundaryPosition != new Vector2(0,0))
        {
            return;
        }

        var camera = GetTree().CurrentScene.GetNodeOrNull<Camera2D>("Camera2D");
        var viewportSize = GetViewport().GetVisibleRect().Size;
        var cameraPos = camera.GlobalPosition;
        var halfViewportHeight = viewportSize.Y / 2;

        UpperBoundaryPosition = new Vector2(cameraPos.X, cameraPos.Y - halfViewportHeight + 10);
        LowerBoundaryPosition = new Vector2(cameraPos.X, cameraPos.Y + halfViewportHeight - 10);
    }

    public void CalculateGoalPositions()
    {
        if (LeftGoalPosition != -1f && RightGoalPosition != -1f)
        {
            return;
        }
        var camera = GetTree().CurrentScene.GetNodeOrNull<Camera2D>("Camera2D");
        var viewPortSize = GetViewport().GetVisibleRect().Size;
        var cameraPos = camera.GlobalPosition;
        var halfViewPortLength = viewPortSize.X / 2;

        LeftGoalPosition = cameraPos.X - halfViewPortLength;
        RightGoalPosition = cameraPos.X + halfViewPortLength;
    }

    public void CalculateMapCenter()
    {
        if (MapCenter != new Vector2(0, 0))
        {
            return;
        }
        var camera = GetTree().CurrentScene.GetNodeOrNull<Camera2D>("Camera2D");
        MapCenter = camera.GlobalPosition;
    }

    private async void CheckForButtonPresses()
    {
        if (Input.IsActionJustPressed("Reset") && StateMachine == StateMachineEnum.Playing)
        {
            LeftPlayerScore = default;
            RightPlayerScore = default;
            Pause = false;
            GetTree().Paused = Pause;
            GetTree().ReloadCurrentScene();
        }
        if (Input.IsActionJustPressed("Pause") && (StateMachine == StateMachineEnum.Playing || StateMachine == StateMachineEnum.InPauseMenu))
        {
            TogglePause();
        }
        if (Input.IsActionJustPressed("Next") && StateMachine == StateMachineEnum.Playing)
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
            SetStateMachine(StateMachineEnum.Playing);
            HidePauseMenu();
        }
    }

    private void ShowPauseMenu()
    {
        GD.Print("showing pause menu");
        _pauseMenu = _pauseMenuScene.Instantiate<PauseMenu>();
        _pauseMenu.ProcessMode = ProcessModeEnum.Always;
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
        InMainMenu = false;
        LeftPlayerScore = default;
        RightPlayerScore = default;
        Pause = false;
        GetTree().Paused = Pause;
        int? newMap = null;
        while (_mapHistory.Contains(newMap) || newMap is null)
        {
            newMap = GD.RandRange(0, _maps.Count - 1);
        }
        _mapHistory.Enqueue(newMap);
        var mapName = _maps[(int)newMap];
        GetTree().ChangeSceneToFile($"res://Scenes/Maps/{mapName}.tscn");

        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); // Wait for new treescene
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); // Wait for nodes to load
    }

    private async void PickRandomMap()
    {
        LockNewRoundWinner = false;
        LeftPlayerScore = default;
        RightPlayerScore = default;
        Pause = false;
        GetTree().Paused = Pause;
        int? newMap = null;
        while (_mapHistory.Contains(newMap) || newMap is null)
        {
            newMap = GD.RandRange(0, _maps.Count - 1);
        }
        _mapHistory.Enqueue(newMap);
        var mapName = _maps[(int)newMap];
        GetTree().ChangeSceneToFile($"res://Scenes/Maps/{mapName}.tscn");
    }
    
    private void GoalHandler(int PlayerScored)
    {
        if ((PlayerEnum)PlayerScored == PlayerEnum.LeftPlayer)
        {
            LeftPlayerScore++;
        }
        else
        {
            RightPlayerScore++;
        }
        CheckForWinner();

        var eventBus = GetNode<Eventbus>(ProngConstants.EventHubPath);
        eventBus.EmitSignal(Eventbus.SignalName.ScoresUpdated, LeftPlayerScore, RightPlayerScore);

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
        GameManager.Instance.SetStateMachine(Shared.StateMachineEnum.InScoreOverview);
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
        GameManager.Instance.SetStateMachine(Shared.StateMachineEnum.Playing);
        PickRandomMap();
    }

    // TODO: Consider moving this logic to GenericMap, so GameManager has nothing to do with balls in the map anymore. 
    private void RemoveAllBalls()
    {
        var balls = GetTree().GetNodesInGroup("Balls");
        foreach (Node ball in balls)
        {
            ball.QueueFree();
        }
    }

    public void SetStateMachine(StateMachineEnum newState)
    {
        StateMachine = newState;
        GD.Print(StateMachine);
    }

}
