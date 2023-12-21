
public partial class HumanPlayer:Player
{

    public override void GobbletClicked(Position pos)
    {
        GameManager.Instance.Gobblet_clicked(pos);
    }

    public override void StartTurn(Player otherPlayer)
    {
        setLegalMoves();
    }
}