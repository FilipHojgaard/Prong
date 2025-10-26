using Godot;

namespace Prong.Src;

public partial class Eventbus : Node
{
    [Signal]
    public delegate void SpeedLevelUpEventHandler(bool Player2);
}

// TODO
// Add this file as Autoload                                                [DONE]
// Emit signal when collecting speed powerup                                [DONE]
// Connect to signal in Prong and have a method to handle powerup.          [DOING]
// Add docs for this event bus pattern in Obsidian. 