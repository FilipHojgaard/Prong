using Godot;
using Prong.Shared;

namespace Prong.Src;

public partial class GenericMap : Node2D
{
    public override void _Ready()
    {
        GameManager.Instance.CalculateGoalPositions();
        GameManager.Instance.CalculateBorderPositions();
        SpawnHorizontalBorders();
        SpawnGoals();
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
}
