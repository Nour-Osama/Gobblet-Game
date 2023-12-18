using System.Collections.Generic;
using Godot;

public class Player
{
    private static readonly int GobbletSizeNum = 4; 
    private static readonly int GobbletNumPerSize= 3; 
    public List<List<Gobblet>> Gobblets;
    public bool white;
    public Gobblet currGobblet;
    public Player(bool white)
    {
        GameBoard g = GameManager.Instance.GameBoard;
        this.white = white;
        Gobblets = new List<List<Gobblet>>();
        for (int i = 0; i < GobbletNumPerSize; i++)
        {
            List<Gobblet> gobblets = new List<Gobblet>();
            for (int j = 0; j < GobbletSizeNum; j++)
            {
                if (this.white)
                {
                    gobblets.Add(new Gobblet(white,j+1,g.getPos(new Position(-2,i))));    
                }
                else
                {
                    gobblets.Add(new Gobblet(white,j+1,g.getPos(new Position(5,i))));
                }
            }
            Gobblets.Add(gobblets);
        }
    }
}