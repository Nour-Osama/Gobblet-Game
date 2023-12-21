
using System;
using System.Collections.Generic;
using Godot;

public class Evaluation
{
    // (size, white)
    // if size = 0 size is ignored 
    private (int s, bool w)[][] evalRows;
    private int[] evalRowsValues;
    private static readonly int evalRowsCount = 10;
    public static readonly int MAX_SCORE = 50000;
    private static readonly int rowIdx = 4;
    private static readonly int posDiagIdx = 8;
    private static readonly int negDiagIdx = 9;
    private int currEval;

    public int CurrEval => currEval;

    private int boardSize;
    public Evaluation(int boardSize)
    {
        this.boardSize = boardSize;
        currEval = 0;
        // initialize eval rows 0 and eval values 0
        evalRows = new (int s, bool w)[evalRowsCount][];
        evalRowsValues = new int[evalRowsCount];
        for (int i = 0; i < evalRowsCount; i++)
        {
            evalRows[i] = new (int s, bool w)[boardSize];
            evalRowsValues[i] = 0;
            for (int j = 0; j < this.boardSize; j++)
            {
                evalRows[i][j] = (0,true);
            }
        }
    }

    private void UpdateCurrEvalValue(int idx)
    {
        // subtract original value
        currEval -= evalRowsValues[idx];
        int whiteSizeSum = 0;
        int blackSizeSum = 0;
        int whiteCount = 0;
        int blackCount = 0;
        // update new value
        foreach (var piece in evalRows[idx])
        {
            if (piece.s > 0)
            {
                if (piece.w)
                {
                    whiteSizeSum += 1;
                    whiteCount += 1;
                }
                else
                {
                    blackSizeSum += 1;
                    blackCount += 1;
                }
            }
        }
        /* eval =
            {
              if C < 4 : C * Sum(S)^2
              else 20,000
            } 
            positive if white, negative if black
            C = partial row count
            S =  gobblet size
         */
        int whiteRowEval = whiteCount < 4 ? whiteCount * whiteSizeSum * whiteSizeSum:MAX_SCORE;
        int blackRowEval = blackCount < 4 ? -1 * blackCount * blackSizeSum * blackSizeSum:-MAX_SCORE;
        evalRowsValues[idx] = whiteRowEval + blackRowEval;
        currEval += evalRowsValues[idx];
    }

    private void UpdateSingleRow(int x, int y, int size, bool w)
    {
        evalRows[x][y] = (size,w);
        UpdateCurrEvalValue(x);
    }
    private void UpdateRows(int x, int y, int size, bool w)
    {
        // update col
        UpdateSingleRow(x, y, size,w);
        // update row
        UpdateSingleRow(y + rowIdx, x, size,w);
        // update +ve diag
        if (x == y)
        {
            UpdateSingleRow(posDiagIdx, y, size,w);
        }
        // update -ve diag
        if ((x + y) == (boardSize-1))
        {
            UpdateSingleRow(negDiagIdx, y, size,w);
        }
    }
    private void UpdateRows(Position pos)
    {
        Gobblet gobblet = pos.GetGobblet();
        // if pos is empty update all positions to be empty
        if (gobblet== null)
        {
            // w param irrelevant since there is no piece
            UpdateRows(pos.x,pos.y,0,false);
        }
        else
        {
            UpdateRows(pos.x,pos.y,gobblet.size,gobblet.white);
        }
    }
    public void UpdateEvalPos(Position originalPos, Position newPos)
    {
        GameBoard g = GameManager.Instance.GameBoard;
        originalPos = g.getPos(originalPos);
        newPos = g.getPos(newPos);
        if (g.IsPositionValid(originalPos))
        {
            UpdateRows(originalPos);
        }
        if (g.IsPositionValid(newPos))
        {
            UpdateRows(newPos);
        }
    }

    public bool GameFinished()
    {
        return Math.Abs(CurrEval) > MAX_SCORE * 0.8;
    }
}