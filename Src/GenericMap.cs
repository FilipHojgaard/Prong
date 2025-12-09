using Godot;
using Prong.Shared;

namespace Prong.Src;

public partial class GenericMap : Node2D
{
    public int BallCount { get; set; } = 0;

    AudioStreamPlayer Music;

    public override void _Ready()
    {
        Music = GetNode<AudioStreamPlayer>("Music");
        Music.Playing = GameManager.MusicOn;

        GameManager.Instance.CalculateGoalPositions();
        GameManager.Instance.CalculateBorderPositions();
        GameManager.Instance.CalculateMapCenter();
        SpawnHorizontalBorders();
        SpawnGoals();

        SpawnInitialBall();
    }

    public override void _EnterTree()
    {
        GetNode<Eventbus>(ProngConstants.EventHubPath).Goal += HandleGoal;
        GetNode<Eventbus>(ProngConstants.EventHubPath).PassBlockBall += HandlePassBLockBall;
        GetNode<Eventbus>(ProngConstants.EventHubPath).RequestBall += HandleRequestBall;
        GetNode<Eventbus>(ProngConstants.EventHubPath).MusicSetting += HandleMusicSetting;
    }

    public override void _ExitTree()
    {
        GetNode<Eventbus>(ProngConstants.EventHubPath).Goal -= HandleGoal;
        GetNode<Eventbus>(ProngConstants.EventHubPath).PassBlockBall -= HandlePassBLockBall;
        GetNode<Eventbus>(ProngConstants.EventHubPath).RequestBall -= HandleRequestBall;
        GetNode<Eventbus>(ProngConstants.EventHubPath).MusicSetting -= HandleMusicSetting;
    }

    public void SpawnHorizontalBorders()
    {
        var borderScene = GD.Load<PackedScene>("res://Scenes/horizontal_border.tscn");
        var upperBorder = borderScene.Instantiate<StaticBody2D>();
        var lowerBorder = borderScene.Instantiate<StaticBody2D>();

        upperBorder.Position = GameManager.UpperBoundaryPosition;
        upperBorder.Rotation = Mathf.Pi;
        lowerBorder.Position = GameManager.LowerBoundaryPosition;

        GetTree().CurrentScene.AddChild(upperBorder);
        GetTree().CurrentScene.AddChild(lowerBorder);

        GD.Print("Horizontal border spawned");
    }

    public void SpawnGoals()
    {
        var goalScene = GD.Load<PackedScene>("res://Scenes/goal.tscn");
        var leftGoal = goalScene.Instantiate<Goal>();
        var rightGoal = goalScene.Instantiate<Goal>();

        leftGoal.Initialize(PlayerEnum.LeftPlayer);
        rightGoal.Initialize(PlayerEnum.RightPlayer);

        leftGoal.Position = new Vector2(GameManager.LeftGoalPosition, 0);
        rightGoal.Position = new Vector2(GameManager.RightGoalPosition, 0);

        GetTree().CurrentScene.AddChild(leftGoal);
        GetTree().CurrentScene.AddChild(rightGoal);

        GD.Print("Goals spawned");
    }

    private async void SpawnInitialBall()
    {
        await ToSignal(GetTree().CreateTimer(1), SceneTreeTimer.SignalName.Timeout);
        SpawnBallAtCenter();
    }

    private void HandleGoal(int EventPlayer)
    {
        BallCount--;
        if (BallCount <= 0)
        {
            SpawnBallAtCenter();
        }
    }

    private void HandlePassBLockBall(Vector2 position, float movementAngle)
    {
        SpawnBallAtPosition(position, movementAngle);
    }

    private void HandleRequestBall(int DeleteBalls)
    {
        for (int i = 0; i < DeleteBalls; i++)
        {
            BallCount--;
        }
        SpawnBallAtCenter();
    }

    public async void SpawnBallAtCenter()
    {
        BallCount++;

        // Fetching the ball scene and instantiating a ball using it. 
        var ballScene = GD.Load<PackedScene>("res://Scenes/ball.tscn");
        var ball = ballScene.Instantiate<Ball>();

        // Because I have a camera on the game scene, I use that to determine the center of the screen for sparning the balls. 
        var camera = GetTree().CurrentScene.GetNodeOrNull<Camera2D>("Camera2D");
        ball.Position = GameManager.MapCenter;

        await ToSignal(GetTree().CreateTimer(0.5), SceneTreeTimer.SignalName.Timeout);

        // Placing the ball on the current scene.
        GetTree().CurrentScene.AddChild(ball);

        // Run ball code when spawned in middle
        ball.SpawnedInMiddle();
    }

    public async void SpawnBallAtPosition(Vector2 position, float rotation)
    {
        BallCount++;

        var ballScene = GD.Load<PackedScene>("res://Scenes/ball.tscn");
        var ball = ballScene.Instantiate<Ball>();

        ball.SpawnInCenter = false;
        ball.StartAtPosition(position, rotation);

        await ToSignal(GetTree().CreateTimer(0.05), SceneTreeTimer.SignalName.Timeout);
        GetTree().CurrentScene.AddChild(ball);
    }

    private void HandleMusicSetting()
    {
        Music.Playing = GameManager.MusicOn;
    }

}
