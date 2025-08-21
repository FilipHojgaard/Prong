using Godot;

namespace Prong.Src;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }
    public static string data { get; set; } = "Lilo&Stitch";
    public override void _Ready()
    {
        Instance = this;
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustReleased("Test"))
        {
            SpawnBall();
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

        GD.Print($"Ball spawend at {ball.Position}");
    }
}
