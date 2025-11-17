using Godot;
using Prong.Shared;

namespace Prong.Src;

public partial class ScoreOverview : Control
{
    public AnimationPlayer AnimationPlayer;
    private Label _PrevLeftScore;
    private Label _PrevRightScore;
    private Label _NewLeftScore;
    private Label _NewtRightScore;

    private PlayerEnum PlayerWonRound;

    public override void _Ready()
    {
        AnimationPlayer = GetNode<AnimationPlayer>("CanvasLayer/AnimationPlayer");
        _PrevLeftScore = GetNode<Label>("CanvasLayer/PreviousLeftOverallScore");
        _PrevRightScore = GetNode<Label>("CanvasLayer/PreviousRightOverallScore");
        _NewLeftScore = GetNode<Label>("CanvasLayer/NewLeftOverallScore");
        _NewtRightScore = GetNode<Label>("CanvasLayer/NewRightOverallScore");
    }

    public void Initialize(PlayerEnum playerWonRound)
    {
        PlayerWonRound = playerWonRound;

        _PrevLeftScore.Text = GameManager.LeftPlayerOverallScorePrevious.ToString();
        _PrevRightScore.Text = GameManager.RightPlayerOverallScorePrevious.ToString();
        _NewLeftScore.Text = GameManager.LeftPlayerOverallScore.ToString();
        _NewtRightScore.Text = GameManager.RightPlayerOverallScore.ToString();
    }

    public void AnimateScores()
    {
        switch (PlayerWonRound)
        {
            case PlayerEnum.Undefined:
                break;
            case PlayerEnum.LeftPlayer:
                AnimationPlayer.Play("left_flip");
                break;
            case PlayerEnum.RightPlayer:
                AnimationPlayer.Play("right_flip");
                break;
            default:
                break;
        }
    }
}
