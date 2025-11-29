using Godot;
using Prong.Shared;

namespace Prong.Src;

public partial class RoundScoreUI : Control
{
    Label LeftPlayerScore;
    Label RightPlayerScore;

    public override void _Ready()
    {
        LeftPlayerScore = GetNode<Label>("PanelContainer/MarginContainer/HBoxContainer/left_score");
        RightPlayerScore = GetNode<Label>("PanelContainer/MarginContainer/HBoxContainer/right_score");

        LeftPlayerScore.Modulate = new Color(1, 1, 1, 0.1f);
        RightPlayerScore.Modulate = new Color(1, 1, 1, 0.1f);
    }
    public override void _EnterTree()
    {
        GetNode<Eventbus>(ProngConstants.EventHubPath).ScoresUpdated += UpdateScores;
        GetNode<Eventbus>(ProngConstants.EventHubPath).EasterEgg += HandleEasterEgg;
    }

    public override void _ExitTree()
    {
        GetNode<Eventbus>(ProngConstants.EventHubPath).ScoresUpdated -= UpdateScores;
        GetNode<Eventbus>(ProngConstants.EventHubPath).EasterEgg -= HandleEasterEgg;
    }

    private void UpdateScores(int LeftPlayerScoreEvent, int RightPlayerScoreEvent)
    {
        LeftPlayerScore.Text = LeftPlayerScoreEvent.ToString();
        RightPlayerScore.Text = RightPlayerScoreEvent.ToString();
    }

    private async void HandleEasterEgg()
    {
        GD.Print("Handle easteregg");
        // TODO: Bug: If someone scored in the 3 seconds, it shows the wrong score. it is better to get the new score from GameManager. 
        // It also breaks the 3 seconds. Consider having it keep showing the easteregg for the 3 seconds using a flag property like i used before with ? : 
        var currentLeftPlayerScore = LeftPlayerScore.Text;
        var currentRightPlayerScore = RightPlayerScore.Text;
        LeftPlayerScore.Text = "28";
        RightPlayerScore.Text = "10";
        await ToSignal(GetTree().CreateTimer(4.0), SceneTreeTimer.SignalName.Timeout);
        LeftPlayerScore.Text = currentLeftPlayerScore;
        RightPlayerScore.Text = currentRightPlayerScore;
    }

    //TODO: Implement Easter Egg 2810 again. 
}
