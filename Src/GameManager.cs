using System.Collections.Generic;
using Godot;

namespace Prong.Src;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }
    public static string data { get; set; } = "Lilo&Stitch";
    public static Vector2 UpperBoundaryPosition { get; set; }
    public static Vector2 LowerBoundaryPosition { get; set; }
    public static float LeftBoundaryPosition { get; set; }
    public static float RightBoundaryPosition { get; set; }
    public static int Player1Score { get; set; } = 0;
    public static int Player2Score { get; set; } = 0;
    public override void _Ready()
    {
        Instance = this;

        CallDeferred(nameof(ConnectToPlayerSignal));

        SpawnHorizontalBorders();
        SetVerticalBorders();
        ConnectToButton();
        ConnectToTimer();
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustReleased("Test"))
        {
            SpawnBall();
        }
    }

    private void ConnectToPlayerSignal()
    {
        var player = GetTree().CurrentScene.GetNodeOrNull<Prong>("Player");

        if (player != null)
        {
            player.HighPosition += OnPlayerHighPosition;
        }
    }

    private void ConnectToButton()
    {
        var button = GetTree().CurrentScene.GetNodeOrNull<Button>("Button");

        if (button != null)
        {
            button.Pressed += () => GD.Print("Button Pressed from GameManager");
        }
    }
    
    private void ConnectToTimer()
    {
        var timer = GetTree().CurrentScene.GetNodeOrNull<Timer>("Timer");

        if (timer != null)
        {
            timer.Timeout += () => GD.Print("Time's up!");
        }
    }

    private void OnPlayerHighPosition()
    {
        GD.Print("listened to event from the Gamemanager");
    }

    public void SpawnHorizontalBorders()
    {
        var borderScene = GD.Load<PackedScene>("res://Scenes/horizontal_border.tscn");
        var upperBorder = borderScene.Instantiate<StaticBody2D>();
        GD.Print("spawned upper Border");
        var lowerBorder = borderScene.Instantiate<StaticBody2D>();
        GD.Print("spawned lower Border");

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

        GD.Print($"Ball spawend at {ball.Position}");
    }

    public static void PrintScore()
    {
        GD.Print($"{Player2Score} - {Player1Score}");
    }
}
