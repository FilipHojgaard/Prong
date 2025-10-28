using Godot;
using Prong.Shared;

namespace Prong.Src;

public partial class Eventbus : Node
{
    [Signal]
    public delegate void SpeedLevelUpEventHandler(int EventPlayer);
}