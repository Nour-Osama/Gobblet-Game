
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
    private static readonly int repetition = 3;
    public static readonly int MAX_SCORE = 50000;
    private static readonly int rowIdx = 4;
    private static readonly int posDiagIdx = 8;
    private static readonly int negDiagIdx = 9;
    private int currEval;
    private bool whiteWon;

    public bool WhiteWon => whiteWon;

    public bool BlackWon => blackWon;

    private bool blackWon;
    private List<GameAction> moves;

    public List<GameAction> Moves => moves;

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
        moves = new List<GameAction>();
        whiteWon = false;
        blackWon = false;
    }

    /*private void CalcCurrEvalSingleRow(int idx)
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
         #1#
        int whiteRowEval = whiteCount < 4 ? whiteCount * whiteSizeSum * whiteSizeSum:MAX_SCORE;
        int blackRowEval = blackCount < 4 ? -1 * blackCount * blackSizeSum * blackSizeSum:-MAX_SCORE;
        evalRowsValues[idx] = whiteRowEval + blackRowEval;
        currEval += evalRowsValues[idx];
    }*/
    
    private (int eval, int whiteCount, int blackCount) CalcEvalSingleRow(int idx)
    {
        int whiteSizeSum = 0;
        int blackSizeSum = 0;
        int whiteCount = 0;
        int blackCount = 0;
        foreach (var piece in evalRows[idx])
        {
            if (piece.s > 0)
            {
                if (piece.w)
                {
                    whiteSizeSum += piece.s;
                    whiteCount += 1;
                }
                else
                {
                    blackSizeSum += piece.s;
                    blackCount += 1;
                }
            }
        }
        /* eval =
            {
              C * Sum(S)^2
            }
            positive if white, negative if black
            C = partial row count
            S =  gobblet size
         */
        int whiteRowEval = whiteCount < 4 ? whiteCount * whiteSizeSum * whiteSizeSum:MAX_SCORE;
        int blackRowEval =  blackCount < 4  ? -1 * blackCount * blackSizeSum * blackSizeSum:-MAX_SCORE;
        return (whiteRowEval + blackRowEval,whiteCount,blackCount);
    }
    
    private int CalcEval(bool whiteTurn)
    {
        int eval = 0;
        whiteWon = false;
        blackWon = false;
        for (int i = 0; i < evalRowsCount; i++)
        {
            (int eval, int whiteCount, int blackCount) rowEval = CalcEvalSingleRow(i);
            if (rowEval.whiteCount >= 4) whiteWon = true; 
            if (rowEval.blackCount >= 4) blackWon = true;
            eval += rowEval.eval;
        }
        // if both won
        if (blackWon && whiteWon)
        {
            // make the player that had the turn lose 
            // that is a special rule that occurs when one moves his gobblet to make 4 in a row 
            // but reveals an opponent gobblet that also makes 4 in a row at the same time
            // in this case the player who moves his piece loses
            if (whiteTurn) whiteWon = false;
            else blackWon = false;
        }
        // else if white won alone
        if (whiteWon && !blackWon)
        {
            return MAX_SCORE;
        }
        // else if black won alone
        if (blackWon && !whiteWon)
        {
            return -1 * MAX_SCORE;
        }
        // otherwise if no one won return current evaluation
        // if draw by repetition condition is true then return 0
        return DrawByRepetition() ? 0:eval;
    }

    private void UpdateSingleRow(int x, int y, int size, bool w)
    {
        evalRows[x][y] = (size,w);
        //CalcCurrEvalSingleRow(x);
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
    public void UpdatePos(Position originalPos, Position newPos, bool anteMove = false)
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
        Gobblet newPosGobblet = newPos.GetGobblet();
        // it was an ante move 
        if (anteMove)
        {
            if (moves.Count > 0)
            {
                moves.RemoveAt(moves.Count-1);
            }
        }
        // else if i it was a real move
        else 
        {
            if(newPosGobblet != null)
                moves.Add(new GameAction(newPosGobblet, newPos, originalPos));
        }
    }
    public void UpdateEval(bool whiteTurn)
    {
        currEval = CalcEval(whiteTurn);
    }

    private bool DrawByRepetition()
    {
        // minimum numbers of move for draw condition to be valid
        if (Moves.Count < repetition * 2 -1 || Moves.Count < 5) return false;
        for (int i = 1; i <= repetition; i++)
        {
           bool moveRepeated =  Moves[^i].NewPos.Equals(Moves[^(i+2)].OldPos) && Moves[^i].OldPos.Equals(Moves[^(i + 2)].NewPos);
           bool sameGobblet = Moves[^i].Gobblet == Moves[^(i + 2)].Gobblet;
           if (!moveRepeated || !sameGobblet) return false;
          // GD.Print(i + " Move repeated");   
        }
         // GD.Print("Draw by repetition");
        return true;
    }
    public bool GameFinished()
    {
        return Math.Abs(currEval) == MAX_SCORE || DrawByRepetition();
    }
}