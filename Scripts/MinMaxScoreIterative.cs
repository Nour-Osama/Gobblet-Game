
using System;
using System.Collections.Generic;
using Godot;

public class MinMaxScoreIterative:MinMaxScorePruning
{
    private long timeLimit;
    private long initialTime;
    public MinMaxScoreIterative(Player white, Player black,int maxDepth,long timeLimit)
        :base(white,black,maxDepth)
    {
        this.timeLimit = timeLimit;
    }

    private long GetElapsedTime()
    {
        long currTime =  DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
        long elapsedTime = currTime - initialTime;
        return elapsedTime;
    }
    protected override void MiniMax(int currDepth,bool whiteTurn)
    {
        initialTime =  DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
        GameAction bestAction = null;
        for (int i = 1; i <= maxDepth; i++)
        {
            base.MiniMax(i,whiteTurn);
            long elapsedTime = GetElapsedTime();
            GD.Print("Current elapsed Time in Milliseconds: " + elapsedTime);
            if (elapsedTime >= timeLimit && i > 1) break;
            // if time limit wasn't reached first iteration is finished then update best action for this iteration
            bestAction = currBestAction;
        }
        // assign best action to last successful depth that is completely finished
        currBestAction = bestAction;
    }
    
    // added time limit constraints 
    protected override int MiniMaxPruning(int depth, int alpha, int beta, bool whiteTurn)
    {
        // if timeout return immediately 
        if (GetElapsedTime() >= timeLimit)
            return evaluation.CurrEval;
        // otherwise operate normally
        return base.MiniMaxPruning(depth, alpha, beta, whiteTurn);
    }
}