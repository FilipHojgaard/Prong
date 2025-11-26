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
    }

    public override void _ExitTree()
    {
        GetNode<Eventbus>(ProngConstants.EventHubPath).ScoresUpdated -= UpdateScores;
    }

    private void UpdateScores(int LeftPlayerScoreEvent, int RightPlayerScoreEvent)
    {
        LeftPlayerScore.Text = LeftPlayerScoreEvent.ToString();
        RightPlayerScore.Text = RightPlayerScoreEvent.ToString();
    }

    //TODO: Implement Easter Egg 2810 again. 
}
