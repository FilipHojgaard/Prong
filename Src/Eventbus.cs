using Godot;

namespace Prong.Src;

public partial class Eventbus : Node
{
    [Signal]
    public delegate void SpeedLevelUpEventHandler(bool Player2);
}