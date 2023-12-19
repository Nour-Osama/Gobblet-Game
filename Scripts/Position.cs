
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using Godot;

public class Position
{
    public int x;
    public int y;
    public Stack<Gobblet> gobbletStack;

    public Position(int x, int y)
    {
        this.x = x;
        this.y = y;
        gobbletStack = new Stack<Gobblet>();
    }

    public Vector2I getVec2i()
    {
        return new Vector2I(x, y);
    }

    public Gobblet GetGobblet()
    {
        return gobbletStack.Count > 0 ? gobbletStack.Peek():null;
    }
    public void PushGobblet(Gobblet gobblet)
    {
        gobbletStack.Push(gobblet);
    }
    public void PopGobblet()
    {
        gobbletStack.Pop();
    }

    public bool Equals(Position pos)
    {
        return x == pos.x && y == pos.y;
    }

    public bool In(List<Position> positions)
    {
        foreach (var pos in positions)
        {
            if (Equals(pos)) return true;
        }
        return false;
    }
}