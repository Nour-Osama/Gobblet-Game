
using System.Collections.Generic;
using Godot;

public class Round
{
    private bool real;
    public static double number = 1;

    public Round(Player player, bool real = true)
    {
        this.player = player;
        this.real = real;
        gobblet = null;
        pos = null;
        moved = false;
    }

    private Gobblet gobblet;
    private Position pos;
    private Player player;
    private bool moved;
    private Position originalPos;
    private List<Position> LegalPositions;
    public Position OriginalPos => originalPos;

    public bool Moved => moved;

    public Gobblet Gobblet => gobblet;

    public Position Pos => pos;

    public Player Player => player;

    public void AttemptToMove(Position pos)
    {
        this.pos = pos;
        if(gobblet != null && GameManager.Instance.GameBoard.IsPositionValid(pos)) moveSequence();
    }
    private void moveSequence()
    {
        if (Pos.In(LegalPositions))
        {
            originalPos = Gobblet.pos;
            Gobblet.move(Pos);
            if (real)
            {
                number += 0.5;
                Gobblet.GobletScene.move(Pos);
            }
            moved = true;
        }
    }

    public void AnteMove()
    {
        if (moved)
        {
            moved = false;
            Gobblet.move(originalPos);
            string originalNotNull = originalPos.GetGobblet() != null ? "true" : "false";
            string posNotNull = pos.GetGobblet() != null ? "true" : "false";
          //  GD.Print("Ante Move to original pos " + originalPos + " not null: " + originalNotNull
         //   + "\tNew pos " + pos + " not null: " + posNotNull);
        }
        else
        {
           // GD.Print("AntiMove FAILED original pos " + originalPos + "\tNew pos " + pos );
        }
    }
    public void SetGobblet(Position pos)
    {
        Gobblet posGobblet = pos.GetGobblet();
        if (posGobblet != null)
        {
            if (posGobblet.white == Player.whiteColor)
            {
                bool firstSelection = Gobblet == null;
                bool externalSelection = posGobblet.isExternal();
                gobblet = firstSelection || externalSelection ? posGobblet:Gobblet;
                LegalPositions = gobblet.GetLegalPositions();
            }
        }
    }

    public void ResetGobblet()
    {
        gobblet = null;
    }
}