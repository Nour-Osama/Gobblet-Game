using Godot;
using System;
using System.Collections.Generic;

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
        pos = GameManager.Instance.GameBoard.getPos(pos);
        // push gobblet to new position 
        pos.PushGobblet(this);
        // pop original position
        this.pos.PopGobblet();
        // change gobblet position 
        this.pos = pos;
    }

    public List<Position> GetLegalPositions()
    {
        GameBoard g = GameManager.Instance.GameBoard;
        List<Position> LegalPositions = new List<Position>();
        // if goblet is exposed then it has legal moves otherwise it doesn't 
        if (pos.GetGobblet() == this)
        {
            // valid positions are from 0 --> height/width since both are the same
            for (int i = 0; i < GameBoard.height; i++)
            {
                for (int j = 0; j < GameBoard.height; j++)
                {
                    Position targetPos = new Position(i, j);
                    Position gameBoardTargetPos = g.getPos(targetPos);
                    Gobblet gobblet = gameBoardTargetPos.GetGobblet();
                    // if condition not necessary should always be valid
                    if (g.IsPositionValid(gameBoardTargetPos))
                    {
                        // if pos empty add it to legal position
                        if (gobblet == null)
                        {
                            LegalPositions.Add(targetPos);
                        }
                        // if position piece had same color and smaller add it to legal position
                        // the gobblet here is implicitly not null without checkking
                        else if (gobblet.white == white && gobblet.size < size)
                        {
                            LegalPositions.Add(targetPos);
                        }
                        //  if position piece had different color and smaller
                        else if (gobblet.white != white && gobblet.size < size)
                        {
                            // if this piece is already on the board add positions to legal positions
                            if (g.IsPositionValid(pos))
                            {
                                LegalPositions.Add(targetPos);
                            }
                            // else if positions piece have at least 3 other pieces of same color in row
                            // add it to legal position
                            else
                            {
                                if (g.checkRow(gameBoardTargetPos, gobblet.white) >= 3)
                                {
                                    LegalPositions.Add(targetPos);
                                }
                            }
                        }
                    }
                }
            }
        }
        return LegalPositions;
    }
    public bool isExternal()
    {
        return !GameManager.Instance.GameBoard.IsPositionValid(pos);
    }
}
