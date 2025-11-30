using Godot;
using Prong.Shared;

namespace Prong.Src;

public partial class RoundScoreUI : Control
{
    public bool ShowEasterEgg { get; set; } = false;

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
        if (ShowEasterEgg)
        {
            return;
        }
        LeftPlayerScore.Text = ShowEasterEgg ? "28" : LeftPlayerScoreEvent.ToString();
        RightPlayerScore.Text = ShowEasterEgg ? "10" : RightPlayerScoreEvent.ToString();
    }

    private async void HandleEasterEgg()
    {
        ShowEasterEgg = true;
        LeftPlayerScore.Text = "28";
        RightPlayerScore.Text = "10";
        await ToSignal(GetTree().CreateTimer(4.0), SceneTreeTimer.SignalName.Timeout);
        LeftPlayerScore.Text = GameManager.LeftPlayerScore.ToString();
        RightPlayerScore.Text = GameManager.RightPlayerScore.ToString();
        ShowEasterEgg = false;
    }
}
