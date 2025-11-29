using Godot;

namespace Prong.Src;

public partial class Border : StaticBody2D
{
    public override void _Ready()
    {
        CollisionLayer = 1;
        CollisionMask = 2;
    }
}
