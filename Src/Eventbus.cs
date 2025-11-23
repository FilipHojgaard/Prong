using Godot;
using Prong.Shared;

namespace Prong.Src;

public partial class Eventbus : Node
{
    [Signal]
    public delegate void SpeedLevelUpEventHandler(int EventPlayer);

    [Signal]
    public delegate void AttackLevelUpEventHandler(int EventPlayer);

    [Signal]
    public delegate void DefenceLevelUpEventHandler(int EventPlayer);

    [Signal]
    public delegate void PassBlockBallEventHandler(Vector2 Position, float MovementAngle);

    [Signal]
    public delegate void GoalEventHandler(int PlayerScored);
}