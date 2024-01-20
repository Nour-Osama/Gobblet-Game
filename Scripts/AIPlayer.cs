
using System.Threading;
using System.Threading.Tasks;

public partial class AIPlayer:Player
{
    private GameAction currBestAction;
    public AbstractMinMaxScore MinMaxScore { set; get; }
    public override void _Process(double delta)
    {
        if (currBestAction != null)
        {
            setLegalMoves();
            GameManager.Instance.Gobblet_clicked(currBestAction.Gobblet.pos);
            GameManager.Instance.Gobblet_clicked(currBestAction.NewPos);
            currBestAction = null;
            MinMaxScore.Reset();
        }
    }

    public override void GobbletClicked(Position pos) { }

    public override void StartTurn(Player otherPlayer)
    { 
        BestAction();
    }

    private async Task BestAction()
    {
        await Task.Delay(500);
        await Task.Run(() =>
        {
            MinMaxScore.CalculateBestMove(whiteColor);
            currBestAction = MinMaxScore.CurrBestAction;
        });
    }
}
