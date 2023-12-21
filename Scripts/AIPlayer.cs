
using System.Threading.Tasks;

public partial class AIPlayer:Player
{
    private GameAction currBestAction;
    private int depth;
    public override void Initialize(bool white, int difficulty)
    {
        base.Initialize(white,difficulty);
        currBestAction = null;
    }

    public override void _Process(double delta)
    {
        if (currBestAction != null)
        {
            setLegalMoves();
            GameManager.Instance.Gobblet_clicked(currBestAction.Gobblet.pos);
            GameManager.Instance.Gobblet_clicked(currBestAction.Pos);
            currBestAction = null;
        }
    }

    public override void GobbletClicked(Position pos) { }

    public override void StartTurn(Player otherPlayer)
    {
        
        Player whitePlayer = whiteColor ? this : otherPlayer;
        Player blackPlayer = whiteColor ? otherPlayer : this;
        BestAction(whitePlayer, blackPlayer);
    }

    private async Task BestAction(Player whitePlayer, Player blackPlayer)
    {
        await Task.Run(() =>
        {
            MinMaxScore minMaxScore = new MinMaxScore(whitePlayer, blackPlayer, whiteColor, difficulty);
            currBestAction = minMaxScore.CurrBestAction;
        });
    }
    // TODO: replace with minmax algorithim later
    // stub function that returns first legal action found
    /*
    private GameAction getAction()
    {
        Gobblet g = null;
        foreach (var gobbletStack in Gobblets)
        {
            foreach (var gobblet in gobbletStack)
            {
                if (gobblet.LegalPositions.Count > 0)
                {
                    g = gobblet;
                    break;
                }
            }
            if (g != null) break;
        }
        Position p = GameManager.Instance.GameBoard.getPos(g.LegalPositions[0]);
        return new GameAction(g, p);
    }*/

}
