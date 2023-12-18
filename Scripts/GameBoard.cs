using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class GameBoard : TileMap
{
	private Position[][] board;

	private static readonly (int min,int max) width = (-2,5);
	private static readonly int height = 4;
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
			   GameManager.Instance.Gobblet_clicked(pos);
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
}
