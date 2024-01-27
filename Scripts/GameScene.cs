using Godot;
using System;

public partial class GameScene : Node2D
{
	[Export] public GameBoard GameBoard;
	
	[Export] public Sprite2D SelectedHalo;
	[Export] public RichTextLabel TurnText;
	public Player whitePlayer;
	public Player blackPlayer;
	// Called when the node enters the scene tree for the first time.
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

	private void SetAiMiniMaxScore( Player player,PlayerOptions playerOptions)
	{
		switch (playerOptions.MinMaxScoreMode)
		{
			case Mode.MiniMax: 
			{
				((AIPlayer)player).MinMaxScore = new MinMaxScore(whitePlayer, blackPlayer, playerOptions.Depth);
				break;
			}
			case Mode.MiniMaxPruning:
			{
				((AIPlayer)player).MinMaxScore = new MinMaxScorePruning(whitePlayer, blackPlayer, playerOptions.Depth);
				break;
			}
			case Mode.MiniMaxIterative:
			{
				((AIPlayer)player).MinMaxScore = new MinMaxScoreIterative(whitePlayer, blackPlayer, playerOptions.Depth,playerOptions.TimeLimit);
				break;
			}
		}
	}
	public override void _Ready()
	{
		GameBoard.Initialize();
		GameManager.Instance.GameBoard = this.GameBoard;
		// initilaize players 
		PlayerOptions whiteOptions = GameManager.Instance.whiteOptions;
		PlayerOptions blackOptions = GameManager.Instance.blackOptions;

		whitePlayer =  whiteOptions.isHuman ? new HumanPlayer(): new AIPlayer();
		blackPlayer =  blackOptions.isHuman ? new HumanPlayer(): new AIPlayer();
		whitePlayer.Initialize(true);
		blackPlayer.Initialize(false);
		if (whitePlayer is AIPlayer)
		{
			SetAiMiniMaxScore(whitePlayer,whiteOptions);
		}
		if (blackPlayer is AIPlayer)
		{
			SetAiMiniMaxScore(blackPlayer,blackOptions);
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

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
