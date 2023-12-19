
public class HumanPlayer:Player
{
    public HumanPlayer(bool white) : base(white)
    {
    }

    public override void GobbletClicked(Position pos)
    {
        GameManager.Instance.Gobblet_clicked(pos);
    }
    public override void StartTurn(){}
}