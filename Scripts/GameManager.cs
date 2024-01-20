using Godot;
using System;
using System.Collections.Generic;
using System.Threading;

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
		
		
		// TODO: replace with gui buttons choices
		whitePlayer = new AIPlayer();
		whitePlayer.Initialize(true);
		blackPlayer = new AIPlayer();
		blackPlayer.Initialize(false);
		if (whitePlayer is AIPlayer)
		{
			((AIPlayer)whitePlayer).MinMaxScore = new MinMaxScoreIterative(whitePlayer, blackPlayer, 13,4);
		}
		if (blackPlayer is AIPlayer)
		{
			((AIPlayer)blackPlayer).MinMaxScore = new MinMaxScoreIterative(whitePlayer, blackPlayer, 13,4);
		}
		
		AddChild(whitePlayer);
		AddChild(blackPlayer);
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
		// initialize round params
		round = new Round(whitePlayer);
		finished = false;
		round.Player.StartTurn(blackPlayer);
		GD.Print("Round " + round + " \tWhite Player turn\n");
	}

	private void checkGameEnded()
	{
		if (GameBoard.Evaluation.GameFinished())
		{
			finished = true;
			if (GameBoard.Evaluation.BlackWon)
			{
				GD.Print("Black Player Won the game");
				TurnText.Text = "Black Player Won the game";
			}
			else if (GameBoard.Evaluation.WhiteWon)
			{
				GD.Print("White Player Won the game");
				TurnText.Text = "White Player Won the game";
			}
			else
			{
				GD.Print("Draw By Repetition");
				TurnText.Text = "Draw By Repetition";
			}
		}
	}

	/*
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
					if (GameBoard.checkWinning(pos, color))
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
			if (round.Player.whiteColor)
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
	}*/
	private void endTurn()
	{
		// evaluate board to determine evaluation score and finishing conditions
		int eval = GameBoard.Evaluate(round.OriginalPos, round.Pos, round.Player.whiteColor);
		// evaluate winning conditions
		GD.Print("Current Board Evaluation " + eval);
		checkGameEnded();
		// reset current gobblet
		ResetCurrGobblet();
		// change round
		Player newRoundPlayer = round.Player == whitePlayer ? blackPlayer : whitePlayer;
		Player otherPlayer = round.Player == whitePlayer ? whitePlayer : blackPlayer;
		round = new Round(newRoundPlayer);
		string color = (round.Player.whiteColor) ? "White": "Black" ;
		if(!finished) TurnText.Text = "Round " + (int)Round.number + "  " + color + "'s turn";
		else TurnText.Text += " after " + (int)Round.number + " Rounds";
		round.Player.StartTurn(otherPlayer);
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
		GD.Print("Pos Clicked " + pos);
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
