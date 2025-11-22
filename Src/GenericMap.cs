using Godot;

namespace Prong.Src;

public partial class GenericMap : Node2D
{
    public override void _Ready()
    {
        GameManager.Instance.CalculateGoalPositions();
        GameManager.Instance.CalculateBorderPositions();
        SpawnHorizontalBorders();
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
}
