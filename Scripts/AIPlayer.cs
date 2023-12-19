
public class AIPlayer:Player
{
    public AIPlayer(bool white) : base(white)
    {
    }

    public override void GobbletClicked(Position pos) { }

    public override void StartTurn()
    {
        (Position g, Position p) action = getAction();
        GameManager.Instance.Gobblet_clicked(action.g);
        GameManager.Instance.Gobblet_clicked(action.p);
    }
    // TODO: replace with minmax algorithim later
    // stub function that returns first legal action found
    private (Position g, Position p) getAction()
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
        return (g.pos, p);
    }
}
