using Godot;

namespace Prong.Src;

public partial class Eventbus : Node
{
    [Signal]
    public delegate void SpeedLevelUp(bool Player2, int newLevel);
}

// TODO
// Add this file as Autoload
// Emit signal when collecting speed powerup
// Connect to signal in Prong and have a method to handle powerup.
// Add docs for this event bus pattern in Obsidian. 