
public class Round
{
    private bool real;
    public static double number = 1;

    public Round(Player player, bool real = true)
    {
        this.player = player;
        this.real = real;
        player.setLegalMoves();
        gobblet = null;
        pos = null;
        moved = false;
    }

    private Gobblet gobblet;
    private Position pos;
    private Player player;
    private bool moved;

    public bool Moved => moved;

    public Gobblet Gobblet => gobblet;

    public Position Pos => pos;

    public Player Player => player;

    public void AttemptToMove(Position pos)
    {
        this.pos = pos;
        if(gobblet != null && GameManager.Instance.GameBoard.IsPositionValid(pos)) moveSequence();
    }
    public void moveSequence()
    {
        if (Pos.In(Gobblet.LegalPositions))
        {
            Gobblet.move(Pos);
            if (real) number += 0.5;
            moved = true;
        }
    }
    public void SetGobblet(Position pos)
    {
        Gobblet posGobblet = pos.GetGobblet();
        if (posGobblet != null)
        {
            if (posGobblet.white == Player.white)
            {
                bool firstSelection = Gobblet == null;
                bool externalSelection = posGobblet.isExternal();
                gobblet = firstSelection || externalSelection ? posGobblet:Gobblet;    
            }
        }
    }

    public void ResetGobblet()
    {
        gobblet = null;
    }
}