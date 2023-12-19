using Godot;
using System;

public partial class GameManager : Node2D
{
	private Player whitePlayer;
	private Player blackPlayer;
	public static GameManager Instance;
	[Export] public GameBoard GameBoard;

	public Round Round => round;

	[Export] public Sprite2D SelectedHalo;
	[Export] public Sprite2D InvalidHalo;
	[Export] public RichTextLabel TurnText;
	private Round round;
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
		whitePlayer = new HumanPlayer(true);
		blackPlayer = new AIPlayer(false);
		// initialize round params
		round = new Round(whitePlayer);
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
	
	private bool checkWinning(Position pos, bool color)
	{
		bool win = false;
		return GameBoard.checkRow(pos,color) == 4;
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
			if (round.Player.white)
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
		// change round
		Player newRoundPlayer = round.Player == whitePlayer ? blackPlayer : whitePlayer;
		round = new Round(newRoundPlayer);
		string color = (round.Player.white) ? "White": "Black" ;
		if(!finished) TurnText.Text = "Round " + (int)Round.number + "  " + color + "'s turn";
		round.Player.StartTurn();
	}
	
	private void setCurrentGobblet()
	{
		Position pos = round.Gobblet.pos;
		Vector2I coords = new Vector2I(pos.x, pos.y);
		Vector2 vec2Pos = GameBoard.MapToLocal(coords);
		SelectedHalo.Set("position", vec2Pos);
		SelectedHalo.Visible = true;
		GD.Print("Current Gobblet is now selected");
	}

	public void Gobblet_clicked(Position pos)
	{
		if (finished) return;
		round.AttemptToMove(pos);
		round.SetGobblet(pos);
		if (round.Gobblet != null) setCurrentGobblet();
		if(round.Moved) endTurn();
	}
	
	private void ResetCurrGobblet()
	{
		round.ResetGobblet();
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
