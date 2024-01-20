using System;
using System.Collections.Generic;
using Godot;

public enum Mode { 
    MiniMax,
    MiniMaxPruning,
    MiniMaxIterative
}
public abstract class AbstractMinMaxScore
{
    protected GameAction currBestAction;

    public GameAction CurrBestAction => currBestAction;

    protected Player white;
    protected Player black;
    protected int maxCurrDepth;
    protected int maxDepth;
    protected int bestEval;
    protected Evaluation evaluation; 
    protected AbstractMinMaxScore(Player white, Player black,  int maxDepth)
    {
        this.white = white;
        this.black = black;
        this.maxDepth = maxDepth;
        this.maxCurrDepth = maxDepth; 
    }

    public void CalculateBestMove(bool whiteTurn)
    {
        // get current board ev
        this.evaluation = GameManager.Instance.GameBoard.Evaluation; 
        GD.Print("Curr Eval before MiniMax: " + evaluation.CurrEval);
        MiniMax(maxCurrDepth,whiteTurn);
        evaluation.UpdateEval(!whiteTurn);
        GD.Print("Curr Eval After MiniMax: " + evaluation.CurrEval);
        GD.Print("Best Evaluation for a depth of " +  this.maxCurrDepth + " is " +bestEval);
        // GD.Print("Best gobblet on " + currBestAction.Gobblet.pos + " and best position is " + currBestAction.NewPos);
    }

    public void Reset()
    {
        currBestAction = null;
    }

    protected abstract void MiniMax(int currDepth,bool whiteTurn);
    protected abstract int MiniMaxCalc(int currDepth,bool whiteTurn);
    
    protected int MinMaxEvalCalc(Player player,int depth, bool whiteTurn, Gobblet gobblet, Position pos)
    {
        Round simRound = new Round(player, false);
        SimulateRound(simRound, gobblet, pos);
        int eval = MiniMaxCalc(depth - 1,!whiteTurn);
        simRound.AnteMove(evaluation);
        return eval;
    }
    protected void SimulateRound(Round simRound,Gobblet gobblet, Position pos)
    {
        simRound.SetGobblet(gobblet.pos);
        simRound.AttemptToMove(pos);
        //  GD.Print("Depth " + depth+" Curr Eval before Move: " + evaluation.CurrEval);
        // update position
        evaluation.UpdatePos(simRound.OriginalPos, simRound.Pos);
        // update eval
        evaluation.UpdateEval(simRound.Player.whiteColor);
        //GD.Print("Depth " + depth+" Curr Eval after Move: " + evaluation.CurrEval);
    }
    
}

