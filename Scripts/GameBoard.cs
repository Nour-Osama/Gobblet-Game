using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class GameBoard : TileMap
{
	private Position[][] board;

	private static readonly (int min,int max) width = (-2,5);
	public static readonly int height = 4;
	// Called when the node enters the scene tree for the first time.
	public void Initialize()
	{
		board = new Position[width.max- width.min +1][];
		for (int i = 0; i <= width.max- width.min; i++)
		{
			board[i] = new Position[height];
			for (int j = 0; j < height; j++)
			{
				board[i][j] = new Position(i+width.min, j);
			}
		}
		// GameManager.Instance.initialize();
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("select"))
		{
           //GD.Print(this.LocalToMap(GetLocalMousePosition()));
		   //GD.Print(this.MapToLocal(LocalToMap(GetLocalMousePosition())));
		   Position pos = getPos(LocalToMap(GetLocalMousePosition()));
		   if (pos != null)
		   {
			   GD.Print("Position:\t(" + pos.x + ", "+ pos.y +")");
			   GameManager.Instance.Round.Player.GobbletClicked(pos);
		   }
		}
	}

	public Position getPos(Vector2 tilePos)
	{
		//GD.Print("Positions:\t(" + tilePos.X + ", "+ tilePos.Y +")");
		if (tilePos.Y < height && tilePos.Y >= 0
		                  && tilePos.X <= width.max && tilePos.X >= width.min)
		{
			return board[(int)tilePos.X - width.min][(int)tilePos.Y];
		}
		return null;
	}
	public Position getPos(Position pos)
	{
		return getPos(new Vector2(pos.x,pos.y));
	}

	public bool IsPositionValid(Position pos)
	{
		if (pos.y < height && pos.y >= 0
		                    && pos.x < height && pos.x >= 0)
		{
			return true;
		}
		return false;
	}
	private bool checkPosition(int x, int y, bool color)
	{
		Position new_pos = getPos(new Position(x, y));
		// if positions is valid and not empty 
		if (IsPositionValid(new_pos) &&
		    new_pos.GetGobblet() != null)
		{
			Gobblet gobblet = new_pos.GetGobblet();
			// if same color
			if (gobblet.white == color)
			{
				return true;
			}
		}
		return false;
	}
		private int checkHorizontal(Position pos, bool color)
	{
		int count = 0;
		int row = pos.x;
		for (int col = 0; col < 4; col++)
		{
			if (checkPosition(row, col, color)) count += 1;
		}
		return count;
	}
	private int checkVertical(Position pos, bool color)
	{
		int count = 0;
		int col = pos.y;
		for (int row = 0; row < 4; row++)
		{
			if (checkPosition(row, col, color)) count += 1;
		}
		return count;
	}
	private Position getAnchorPos(Position pos,int xMod, int yMod)
	{
		Position anchor_pos = pos;
		int count = 0;
		for (int i = 0; i < 4; i++)
		{
			// get new diagonal position before current
			Position new_pos = new Position(pos.x + xMod * (i + 1), pos.y + yMod * (i+1));
			// if position is valid
			if (IsPositionValid(new_pos))
			{
				// assign new position
				anchor_pos = new_pos;
			}
			// if position not valid return current anchor count
			else
			{
				return anchor_pos;
			}
		}
		return anchor_pos;
	}
	
	private int getAnchorPosCount(Position pos,int xMod, int yMod,bool color)
	{
		int count = checkPosition(pos.x, pos.y, color) ? 1:0;
		for (int i = 0; i < 4; i++)
		{
			// get new diagonal position before current
			Position new_pos = new Position(pos.x + xMod * (i + 1), pos.y + yMod * (i+1));
			// if position is valid
			if (IsPositionValid(new_pos))
			{
				if (checkPosition(new_pos.x, new_pos.y, color)) count += 1;
			}
			// if position not valid return current anchor count
			else
			{
				return count;
			}
		}
		return count;
	}
	private int checkDiagonal(Position pos, bool color)
	{
		Position positive_anchor_pos = getAnchorPos(pos, 1, -1);
		Position neg_anchor_pos = getAnchorPos(pos, -1, -1);
		int pos_anchor_count = getAnchorPosCount(positive_anchor_pos,-1,1,color);
		int neg_anchor_count = getAnchorPosCount(neg_anchor_pos,1,1,color);
		return pos_anchor_count > neg_anchor_count ? pos_anchor_count:neg_anchor_count;
	}
	public int checkRow(Position pos, bool color)
	{
		int count = 0;
		int max_count = count;
		count = checkHorizontal(pos, color);
		max_count = max_count < count ? count : max_count;
		// check vertical
		count = checkVertical(pos, color);
		max_count = max_count < count ? count : max_count;
		// check diagonal
		count = checkDiagonal(pos, color);
		max_count = max_count < count ? count : max_count;
		return max_count;
	}
}
