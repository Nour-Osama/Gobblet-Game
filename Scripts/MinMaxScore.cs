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
        evaluation.UpdateEval(!whiteTurn);
        GD.Print("Curr Eval After MiniMax: " + evaluation.CurrEval);
        GD.Print("Best Evaluation for a depth of " + depth + " is " +bestEval);
        GD.Print("Best gobblet on " + currBestAction.Gobblet.pos + " and best position is " + currBestAction.NewPos);
    }
    private void SimulateRound(Round simRound,Gobblet gobblet, Position pos)
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
    private int MinMaxEvalCalc(Player player,int depth, bool whiteTurn, Gobblet gobblet, Position pos,
        int alpha = 0, int beta = 0,bool alphaBeta = true)
    {
        Round simRound = new Round(player, false);
        SimulateRound(simRound, gobblet, pos);
        if (alphaBeta)
        {
            int eval = MiniMaxPruning(depth - 1, alpha, beta, !whiteTurn);
            simRound.AnteMove(evaluation);
            return eval;
        }
        else
        {
            int eval = MiniMax(depth - 1, !whiteTurn);
            simRound.AnteMove(evaluation);
            return eval;
        }
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
                        var eval = MinMaxEvalCalc(white,depth,whiteTurn, gobblet, pos, alpha, beta);
                       /* if (alpha == eval)
                        {
                            changeBestAction(depth,white, gobblet, pos);
                        }*/
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
            foreach (var gobbletStack in black.Gobblets)
            {
                foreach (var gobblet in gobbletStack)
                {
                    List<Position> LegalPositions = gobblet.GetLegalPositions();
                    foreach (var pos in LegalPositions)
                    {
                        var eval = MinMaxEvalCalc(black,depth, whiteTurn, gobblet, pos,alpha, beta);
                        /*if (beta == eval)
                        {
                            changeBestAction(depth,black, gobblet, pos); 
                        }*/
                        if (beta > eval)
                        {
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

    private void changeBestAction(int depth,Player player, Gobblet gobblet, Position pos)
    {
        if (this.depth == depth)
        {
            Round currBestActionRound = new Round(player, false);
            SimulateRound(currBestActionRound, currBestAction.Gobblet, currBestAction.NewPos);
            int currBestEval = evaluation.CurrEval;
            currBestActionRound.AnteMove(evaluation);
            Round currActionRound = new Round(player, false);
            SimulateRound(currActionRound, gobblet, pos);
            int currEval = evaluation.CurrEval;
            currActionRound.AnteMove(evaluation);
            if (currEval > currBestEval && player.whiteColor)
            {
                GD.Print("CURR BEST EVAL after 1 move is " + currBestEval + " for move " + currBestAction);
                currBestAction = new GameAction(gobblet, pos);
                GD.Print("Eval for new move is " + currEval + " for move " + currBestAction);
            }
            else if (currEval < currBestEval && !player.whiteColor)
            {
                GD.Print("CURR BEST EVAL after 1 move is " + currBestEval + " for move " + currBestAction);
                currBestAction = new GameAction(gobblet, pos);
                GD.Print("Eval for new move is " + currEval + " for move " + currBestAction);
            }
        }
    }

    private int MiniMax(int depth, bool whiteTurn)
    {
        // base case : max depth or game finished
        if (depth == 0 || evaluation.GameFinished())
        {
            return evaluation.CurrEval;
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
                        int eval = MinMaxEvalCalc(white, depth, whiteTurn, gobblet, pos, alphaBeta: false);
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
                        int eval = MinMaxEvalCalc(black, depth, whiteTurn, gobblet, pos, alphaBeta: true);
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