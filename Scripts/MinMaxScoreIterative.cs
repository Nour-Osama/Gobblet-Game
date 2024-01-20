
using System;
using Godot;

public class MinMaxScoreIterative:MinMaxScorePruning
{
    private long timeLimit;
    public MinMaxScoreIterative(Player white, Player black,int maxDepth,long timeLimit)
        :base(white,black,maxDepth)
    {
        this.timeLimit = timeLimit;
    }
    protected override void MiniMax(int currDepth,bool whiteTurn)
    {
        long initialTime =  DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
        for (int i = 1; i <= maxDepth; i++)
        {
            base.MiniMax(i,whiteTurn);
            long currTime =  DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
            long elapsedTime = currTime - initialTime;
            GD.Print("Current elapsed Time in Milliseconds: " + elapsedTime);
            if (elapsedTime >= timeLimit) break;
        }
    }
}