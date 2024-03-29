

using System;
using System.Collections.Generic;
using Godot;

public class MinMaxScore:AbstractMinMaxScore
{
    public MinMaxScore(Player white, Player black, int maxDepth)
    :base(white,black,maxDepth){}

    protected override void MiniMax(int currDepth, bool whiteTurn)
    {
        bestEval = MiniMaxCalc(currDepth, whiteTurn);
    }

    protected override int MiniMaxCalc(int depth, bool whiteTurn)
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
                        int eval = MinMaxEvalCalc(white, depth, whiteTurn, gobblet, pos);
                        if (maxEval < eval)
                        {
                            maxEval = eval;
                            if (this.maxCurrDepth == depth)
                            {
                                GD.Print("CURR BEST EVAL: " + eval);
                                currBestAction = new GameAction(gobblet,pos);
                            }
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
                        int eval = MinMaxEvalCalc(black, depth, whiteTurn, gobblet, pos);
                        if (minEval > eval)
                        {
                            minEval = eval;
                            if (this.maxCurrDepth == depth)
                            {
                                GD.Print("CURR BEST EVAL: " + eval);
                                currBestAction = new GameAction(gobblet,pos);
                            }
                        }                    
                    }
                }
            }
            return minEval;
        }
    }
}