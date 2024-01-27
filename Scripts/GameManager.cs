using Godot;
using System;
using System.Collections.Generic;
using System.Threading;

public partial class GameManager : Node2D
{
	public static GameManager Instance;
	public GameBoard GameBoard;
	[Export] private PackedScene gamePackedScene;
	private GameScene gameInstance;
	public Round Round => round;
	[Export] public PlayerOptions whiteOptions;
	[Export] public PlayerOptions blackOptions;
	[Export] private Button newGameBut;
	[Export] private Button exitBut;
	private Sprite2D SelectedHalo;
	private RichTextLabel TurnText;
	private Round round;
	private bool finished;

	// Called when the node enters the scene tree for the first time.

	private void StartNewGame()
	{
		// delete Past Game
		if(gameInstance != null) gameInstance.QueueFree();
		// reset round counter
		Round.number = 1;
		// start new game
		gameInstance = gamePackedScene.Instantiate<GameScene>();
		AddChild(gameInstance);
		SelectedHalo = gameInstance.SelectedHalo;
		TurnText = gameInstance.TurnText;
		// initialize round params
		round = new Round(gameInstance.whitePlayer);
		finished = false;
		round.Player.StartTurn(gameInstance.blackPlayer);
		GD.Print("Round " + round + " \tWhite Player turn\n");
	} 
	
	public override void _Ready()
	{
		// intiaize gamemanager instance
		Instance = this;
		newGameBut.Pressed += StartNewGame;
		exitBut.Pressed += ExitGame;
	}

	private void ExitGame()
	{
		GetTree().Quit();
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

	private void endTurn()
	{
		Player whitePlayer = gameInstance.whitePlayer;
		Player blackPlayer = gameInstance.blackPlayer;
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
