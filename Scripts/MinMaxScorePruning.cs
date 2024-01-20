
using System.Collections.Generic;
using Godot;

public class MinMaxScorePruning: MinMaxScore
{
    public MinMaxScorePruning(Player white, Player black, int maxDepth)
    :base(white,black,maxDepth)
    {
    }
    protected override void MiniMax(int currDepth,bool whiteTurn)
    {
        int alpha = int.MinValue;
        int beta = int.MaxValue;
        maxCurrDepth = currDepth;
        bestEval = MiniMaxPruning(maxCurrDepth,alpha,beta,whiteTurn);
    }
     private int MinMaxEvalCalc(Player player,int depth, bool whiteTurn, Gobblet gobblet, Position pos,
        int alpha , int beta )
    {
        Round simRound = new Round(player, false);
        SimulateRound(simRound, gobblet, pos);
        int eval = MiniMaxPruning(depth - 1, alpha, beta, !whiteTurn);
        simRound.AnteMove(evaluation);
        return eval;
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
            foreach (var gobbletStack in white.Gobblets)
            {
                foreach (var gobblet in gobbletStack)
                {
                    List<Position> LegalPositions = gobblet.GetLegalPositions();
                    foreach (var pos in LegalPositions)
                    {
                        var eval = MinMaxEvalCalc(white,depth,whiteTurn, gobblet, pos, alpha, beta);

                        if (alpha < eval)
                        {
                            alpha = eval;
                            if (this.maxCurrDepth == depth)
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
                        if (beta > eval)
                        {
                            beta = eval;
                            if (this.maxCurrDepth == depth)
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

}