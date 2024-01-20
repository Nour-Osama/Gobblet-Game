using System.Collections.Generic;
using Godot;

public abstract partial class Player:Node2D
{
    private static readonly int GobbletSizeNum = 4; 
    private static readonly int GobbletNumPerSize= 3; 
    public List<List<Gobblet>> Gobblets;
    public bool whiteColor;
    public override void _Ready()
    {
        
    }

    public void Initialize(bool whiteColor)
    {
        GameBoard g = GameManager.Instance.GameBoard;
        this.whiteColor = whiteColor;
        Gobblets = new List<List<Gobblet>>();
        for (int i = 0; i < GobbletNumPerSize; i++)
        {
            List<Gobblet> gobblets = new List<Gobblet>();
            for (int j = 0; j < GobbletSizeNum; j++)
            {
                if (this.whiteColor)
                {
                    gobblets.Add(new Gobblet(whiteColor,j+1,g.getPos(new Position(-2,i))));    
                }
                else
                {
                    gobblets.Add(new Gobblet(whiteColor,j+1,g.getPos(new Position(5,i))));
                }
            }
            Gobblets.Add(gobblets);
        }
    }

    public void setLegalMoves()
    {
        foreach (var gobbletStack in Gobblets)
        {
            foreach (var gobblet in gobbletStack)
            {
                gobblet.GetLegalPositions();
            }
        }
    }

    public abstract void GobbletClicked(Position pos);
    public abstract void StartTurn(Player otherPlayer);
}