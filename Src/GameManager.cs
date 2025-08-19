using Godot;

namespace Prong.Src;

public partial class GameManager : Node
{
    public GameManager Instance { get; private set; }
    public static string data { get; set; } = "Lilo&Stitch";
    public override void _Ready()
    {
        Instance = this;
    }
}
