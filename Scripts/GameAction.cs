
public class GameAction
{
    private Gobblet gobblet;
    private Position _newPos;
    private Position _oldPos;

    public Position OldPos
    {
        get => _oldPos;
        set => _oldPos = value;
    }


    public Gobblet Gobblet
    {
        get => gobblet;
    }

    public Position NewPos
    {
        get => _newPos;
    }

    public GameAction(Gobblet gobblet, Position newPos, Position oldPos)
    {
        this.gobblet = gobblet;
        _newPos = newPos;
        _oldPos = oldPos;
    }

    public GameAction(Gobblet gobblet, Position newPos)
    {
        this.gobblet = gobblet;
        this._newPos = newPos;
    }
    public override string ToString()
    {
        if(_oldPos != null)
            return "gobblet: " + gobblet + " Old pos: "  + _oldPos.ToString() + " New pos"+ _newPos.ToString();
        return "gobblet: " + gobblet +  " New pos"+ _newPos.ToString();
    }
}