
public class GameAction
{
    private Gobblet gobblet;
    private Position pos;

    public Gobblet Gobblet
    {
        get => gobblet;
        set => gobblet = value;
    }

    public Position Pos
    {
        get => pos;
        set => pos = value;
    }

    public GameAction(Gobblet gobblet, Position pos)
    {
        this.gobblet = gobblet;
        this.pos = pos;
    }

}