using Godot;
using System;

public partial class Gobblet
{
    public PackedScene GobbletScene;
    public GobletScene GobletScene;
    //public AbstractGobbletScene AbstractGobbletScene;
    public bool white;
    public int size;
    public Position pos;

    public Gobblet(bool white, int size, Position pos)
    {
        GobbletScene = GD.Load<PackedScene>("res://Scenes/goblet" + size +".tscn");
        this.pos = pos;
        this.white = white;
        this.size = size;
        pos.PushGobblet(this);
    }

    public void move(Position pos)
    {
        // push gobblet to new position 
        pos.PushGobblet(this);
        // pop original position
        this.pos.PopGobblet();
        // change gobblet position visually and in code
        this.pos = pos;
        GobletScene.move(pos);
    }

    public bool isExternal()
    {
        return !GameManager.Instance.GameBoard.IsPositionValid(pos);
    }
}
