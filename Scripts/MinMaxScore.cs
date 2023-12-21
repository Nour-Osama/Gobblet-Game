using System;
using System.Collections.Generic;
using Godot;

public class MinMaxScore
{
    private GameAction currBestAction;

    public GameAction CurrBestAction => currBestAction;

    private Player white;
    private Player black;
    private int depth;
    private int bestEval;
    private Evaluation evaluation; 
    public MinMaxScore(Player white, Player black, bool whiteTurn, int depth = 1)
    {
        this.white = white;
        this.black = black;
        this.depth = depth;
        int alpha = int.MinValue;
        int beta = int.MaxValue;
        // get current board ev
        this.evaluation = GameManager.Instance.GameBoard.Evaluation; 
        GD.Print("Curr Eval before MiniMax: " + evaluation.CurrEval);
        bestEval = MiniMaxPruning(depth,alpha,beta,whiteTurn);
        //bestEval = MiniMax(depth,whiteTurn,false);
        GD.Print("Curr Eval After MiniMax: " + evaluation.CurrEval);
        GD.Print("Best Evaluation for a depth of " + depth + " is " +bestEval);
        GD.Print("Best gobblet on " + currBestAction.Gobblet.pos + " and best position is " + currBestAction.Pos);
    }
    private int MiniMaxPruning(int depth,int alpha, int beta, bool whiteTurn)
    {
        // base case : max depth or game finished
        if (depth == 0 || evaluation.GameFinished())
        {
            return evaluation.CurrEval;
        }
        if (whiteTurn)
        {
            //white.setLegalMoves();
            foreach (var gobbletStack in white.Gobblets)
            {
                foreach (var gobblet in gobbletStack)
                {
                    List<Position> LegalPositions = gobblet.GetLegalPositions();
                    foreach (var pos in LegalPositions)
                    {
                     //   GD.Print("Depth " + depth+" Curr Eval before Move: " + evaluation.CurrEval);
                        Round simRound = new Round(white, false);
                        simRound.SetGobblet(gobblet.pos);
                        simRound.AttemptToMove(pos);
                        //gameFinished = GameManager.Instance.GameBoard.checkWinning(pos, whiteTurn);
                        evaluation.UpdateEvalPos(simRound.OriginalPos,simRound.Pos);
                     //   GD.Print("Depth " + depth+" Curr Eval after Move: " + evaluation.CurrEval);
                        int eval = MiniMaxPruning(depth - 1, alpha,beta,!whiteTurn);
                      //  GD.Print("val after Move: " + eval);
                        simRound.AnteMove();
                        evaluation.UpdateEvalPos(simRound.OriginalPos,simRound.Pos);
                     //   GD.Print("Depth " + depth+" Curr Eval after AnteMove: " + evaluation.CurrEval);
                        if (alpha < eval)
                        {
                            alpha = eval;
                            if (this.depth == depth)
                            {
                                GD.Print("CURR BEST EVAL: " + eval);
                                currBestAction = new GameAction(gobblet,pos);
                            }
                        }
                        if (beta<= alpha) return alpha;
                    }
                }
            }
            return alpha;
        }
        else
        {
            black.setLegalMoves();
            foreach (var gobbletStack in black.Gobblets)
            {
                foreach (var gobblet in gobbletStack)
                {
                    List<Position> LegalPositions = gobblet.GetLegalPositions();
                    foreach (var pos in LegalPositions)
                    {
                       // GD.Print("Depth " + depth+" Curr Eval before Move: " + evaluation.CurrEval);
                        Round simRound = new Round(black, false);
                        simRound.SetGobblet(gobblet.pos);
                        simRound.AttemptToMove(pos);
                        evaluation.UpdateEvalPos(simRound.OriginalPos,simRound.Pos);
                        //gameFinished = GameManager.Instance.GameBoard.checkWinning(pos, whiteTurn);
                      //  GD.Print("Depth " + depth+" Curr Eval after Move: " + evaluation.CurrEval);
                        int eval = MiniMaxPruning(depth - 1, alpha,beta,!whiteTurn);
                       // GD.Print("Curr Eval after Move: " + evaluation.CurrEval);
                        simRound.AnteMove();
                        evaluation.UpdateEvalPos(simRound.OriginalPos,simRound.Pos);
                      //  GD.Print("Depth " + depth+" Curr Eval after AnteMove: " + evaluation.CurrEval);
                        if (beta > eval)
                        {
                    //        GD.Print("CURR BEST EVAL: " + eval);
                            beta = eval;
                            if (this.depth == depth)
                            {
                                GD.Print("CURR BEST EVAL: " + eval);
                                currBestAction = new GameAction(gobblet,pos);
                            }
                        } 
                        if (beta<= alpha) return beta;
                    }
                }
            }
            return beta;
        }
    }
    private int MiniMax(int depth, bool whiteTurn, bool gameFinished)
    {
        GameBoard g = GameManager.Instance.GameBoard;
        // base case
        if (depth == 0 || gameFinished)
        {
            return g.EvaluateNaive();
        }
        if (whiteTurn)
        {
            int maxEval = Int32.MinValue;
            foreach (var gobbletStack in white.Gobblets)
            {
                foreach (var gobblet in gobbletStack)
                {
                    List<Position> LegalPositions = gobblet.GetLegalPositions();
                    foreach (var pos in LegalPositions)
                    {
                        Round simRound = new Round(white, false);
                        simRound.SetGobblet(gobblet.pos);
                        simRound.AttemptToMove(pos);
                        gameFinished = GameManager.Instance.GameBoard.checkWinning(pos, whiteTurn);
                        int eval = MiniMax(depth - 1, !whiteTurn,gameFinished);
                        simRound.AnteMove();
                        if (maxEval < eval)
                        {
                            maxEval = eval;
                            if(this.depth == depth)currBestAction = new GameAction(gobblet,pos);
                        }
                    }
                }
            }
            return maxEval;
        }
        else
        {
            int minEval = Int32.MaxValue;
            foreach (var gobbletStack in black.Gobblets)
            {
                foreach (var gobblet in gobbletStack)
                {
                    List<Position> LegalPositions = gobblet.GetLegalPositions();
                    foreach (var pos in LegalPositions)
                    {
                        Round simRound = new Round(black, false);
                        simRound.SetGobblet(gobblet.pos);
                        simRound.AttemptToMove(pos);
                        gameFinished = GameManager.Instance.GameBoard.checkWinning(pos, whiteTurn);
                        int eval = MiniMax(depth - 1, !whiteTurn,gameFinished);
                        simRound.AnteMove();
                        if (minEval > eval)
                        {
                            minEval = eval;
                            if(this.depth == depth)currBestAction = new GameAction(gobblet,pos);
                        }                    
                    }
                }
            }
            return minEval;
        }
    }
}