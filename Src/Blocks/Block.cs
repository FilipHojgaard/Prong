using Godot;

namespace Prong.Src.Blocks;

public partial class Block : RigidBody2D
{
    public override void _Ready()
    {
        GravityScale = 0;
        Mass = 1000;
    }
}
