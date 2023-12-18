using Godot;
using System;

public partial class GameManager : Node2D
{
	private Player whitePlayer;
	private Player blackPlayer;
	public static GameManager Instance;
	[Export] public GameBoard GameBoard;
	[Export] public Sprite2D SelectedHalo;
	[Export] public Sprite2D InvalidHalo;
	[Export] public RichTextLabel TurnText;
	private Player currPlayer;
	private double round;
	private bool finished;
	private void instantiateGobblet(Gobblet gobblet)
	{
		var instance = gobblet.GobbletScene.Instantiate() as GobletScene;
		string color = (gobblet.white) ? "White": "Black" ;
		string size = (gobblet.size > 2) ? "big": "small" ;
		string path = $"res://Assets/Gobblets/{color}/{size}.tres";
		instance.Sprite2D.Texture = GD.Load<AtlasTexture>(path);
		instance.Name = "gobblet " + gobblet.size;
		gobblet.GobletScene = instance;
		AddChild(instance);
		Vector2 pos = GameBoard.MapToLocal(gobblet.pos.getVec2i());
		//GD.Print(pos);
		instance.Set("position", pos);
	}
	// Called when the node enters the scene tree for the first time.
	
	public override void _Ready()
	{
		// initialize gameboard
		GameBoard.Initialize();
		// intiaize gamemanager instance
		Instance = this;
		// initilaize players 
		whitePlayer = new Player(true);
		blackPlayer = new Player(false);
		// initialize round params
		currPlayer = whitePlayer;
		currPlayer.currGobblet = null;
		round = 1;
		finished = false;
		GD.Print("Round " + round + " \tWhite Player turn\n");
		foreach (var gobbletType in whitePlayer.Gobblets)
		{
			foreach (var gobblet in gobbletType)
			{
				instantiateGobblet(gobblet);
			}
		}
		
		foreach (var gobbletSize in blackPlayer.Gobblets)
		{
			foreach (var gobblet in gobbletSize)
			{
				instantiateGobblet(gobblet);
			}
		}
	}

	private bool checkPosition(int x, int y, bool color)
	{
		Position new_pos = GameBoard.getPos(new Position(x, y));
		// if positions is valid and not empty 
		if (GameBoard.IsPositionValid(new_pos) &&
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
			if (GameBoard.IsPositionValid(new_pos))
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
			if (GameBoard.IsPositionValid(new_pos))
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
	private int checkRow(Position pos, bool color)
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
	private bool checkWinning(Position pos, bool color)
	{
		bool win = false;
		return checkRow(pos,color) == 4;
	}
	private void checkGameEnded()
	{
		bool whiteWon = false;
		bool blackWon = false;
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				// for each valid positions
				Position pos = GameBoard.getPos(new Position(i, j));
				// if not empty
				if (pos.GetGobblet() != null)
				{
					bool color = pos.GetGobblet().white;
					if (checkWinning(pos, color))
					{
						finished = true;
						if (pos.GetGobblet().white)
						{
							whiteWon = true;
						}
						else blackWon = true;
					}
				}
			}	
		}
		// if after movement both players won then the current player that moved piece loses
		if (blackWon && whiteWon)
		{
			if (currPlayer.white)
			{
				whiteWon = false;
				GD.Print("Black Player Won the game");
				TurnText.Text = "Black Player Won the game";
			}
			else
			{
				blackWon = false;
			    GD.Print("White Player Won the game");
			    TurnText.Text = "White Player Won the game";
			}
		}
		else if (blackWon)
		{
			GD.Print("Black Player Won the game");
			TurnText.Text = "Black Player Won the game";
		}
		else if (whiteWon)
		{
			GD.Print("White Player Won the game");
			TurnText.Text = "White Player Won the game";
		}
	}
	private void endTurn()
	{
		// evaluate winning conditions
		checkGameEnded();
		// reset current gobblet
		ResetCurrGobblet();
		// change player
		currPlayer = currPlayer == whitePlayer ? blackPlayer : whitePlayer;
		// add round each two turns
		round += 0.5;
		string color = (currPlayer.white) ? "White": "Black" ;
		if(!finished) TurnText.Text = "Round " + (int)round + "  " + color + "'s turn";
		GD.Print("Round " + round + " " + color + " \tPlayer turn\n");
	}

	private void setCurrentGobblet(Gobblet targetGobblet)
	{
		currPlayer.currGobblet = targetGobblet;
		Position pos = currPlayer.currGobblet.pos;
		Vector2I coords = new Vector2I(pos.x, pos.y);
		Vector2 vec2Pos = GameBoard.MapToLocal(coords);
		SelectedHalo.Set("position", vec2Pos);
		SelectedHalo.Visible = true;
		GD.Print("Current Gobblet is now selected");
	}
	public void Gobblet_clicked(Position pos)
	{
		if (finished) return;
		Gobblet targetGobblet = pos.GetGobblet();
		// if position isn't empty
		if (targetGobblet != null)
		{
			// if no gobblet was selected this turn and he clicked on his own gobblet
			if (currPlayer.currGobblet == null && targetGobblet.white == currPlayer.white)
			{
				// set current gobblet
				setCurrentGobblet(targetGobblet);
			}
			// if a gobblet was already selected
			else if(currPlayer.currGobblet != null)
			{
				// if gobblet selected is same color 
				if (targetGobblet.white == currPlayer.white)
				{
					// if target gobblet size bigger than current size or gobblet is external
					if (targetGobblet.size > currPlayer.currGobblet.size || targetGobblet.isExternal())
					{
						// change current gobblet
						setCurrentGobblet(targetGobblet);					
					}
					else
					{								
						// if target gobblet size is less than current gobblet size 
						if (targetGobblet.size < currPlayer.currGobblet.size)
						{
							currPlayer.currGobblet.move(pos);
							endTurn();
						}
					}
				}
				// else if goblet position was valid
				else if (GameBoard.IsPositionValid(pos))
				{
					// if target goblet is the opponent's 
					if (targetGobblet.white != currPlayer.white)
					{
						GD.Print("opponent gobblet clicked!");
						// if curr gobblet is external
						if (currPlayer.currGobblet.isExternal())
						{
							// check if their are at least 3 opposing gobblets in a row that has target gobblet
							if (checkRow(targetGobblet.pos, targetGobblet.white) >= 3)
							{
								// if target gobblet size is less than current gobblet size 
								if (targetGobblet.size < currPlayer.currGobblet.size)
								{
									currPlayer.currGobblet.move(pos);
									endTurn();
								}
							}
						}
						else
						{								
							// if target gobblet size is less than current gobblet size 
							if (targetGobblet.size < currPlayer.currGobblet.size)
							{
								currPlayer.currGobblet.move(pos);
								endTurn();
							}
						}
					}
					else
					{
						// if target gobblet size is less than current gobblet size 
						if (targetGobblet.size < currPlayer.currGobblet.size)
						{
							currPlayer.currGobblet.move(pos);
							endTurn();
						}
					}
				}
				
			}
		}
		// if curr goblett was selected && position was empty && position was valid
		else if (currPlayer.currGobblet != null && targetGobblet == null && GameBoard.IsPositionValid(pos))
		{
			currPlayer.currGobblet.move(pos);
			endTurn();
		}
	}

	private void ResetCurrGobblet()
	{
		currPlayer.currGobblet = null;
		GD.Print("Current gobblet reset");
		SelectedHalo.Visible = false;
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("cancel"))
		{
			ResetCurrGobblet();
		}
	}
}
