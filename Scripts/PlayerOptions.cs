using Godot;
using System;

public partial class PlayerOptions : MarginContainer
{
	[Export] private OptionButton PlayerType;
	[Export] private HBoxContainer AiOptions;
	[Export] private OptionButton miniMaxType;
	[Export] private SpinBox depth; 
	[Export] private SpinBox timeLimit;
	[Export] private HBoxContainer timeLimitHbox;
	public int Depth { get; set; }
	public int TimeLimit { get; set; }
	public Mode MinMaxScoreMode { get; set; }

	public bool isHuman;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PlayerType.ItemSelected += HandlePlayerTypeSelected;
		miniMaxType.ItemSelected += HandleMiniMaxTypeSelected;
		depth.ValueChanged += HandleDepthChanged;
		timeLimit.ValueChanged += HandleTimeLimitChanged;
		// default values
		isHuman = true;
		Depth = (int)depth.Value;
		TimeLimit = (int)timeLimit.Value;
		MinMaxScoreMode = Mode.MiniMaxPruning;
	}

	private void HandleTimeLimitChanged(double newVal)
	{
		GD.Print("Time limit changed to " + newVal);
		TimeLimit = (int)newVal;
	}
	private void HandleDepthChanged(double newVal)
	{
		GD.Print("Depth changed to " + newVal);
		Depth = (int)newVal;
	}
	private void HandlePlayerTypeSelected(long selected)
	{
		if (selected == 0)
		{
			GD.Print("Human pLayer Now");
			isHuman = true;
			AiOptions.Visible = false;
		}
		else
		{
			GD.Print("Ai pLayer Now");
			isHuman = false;
			AiOptions.Visible = true;
		}
	}

	private void HandleMiniMaxTypeSelected(long selected)
	{
		switch (selected)
		{
			case 0:
			{
				MinMaxScoreMode = Mode.MiniMax;
				timeLimitHbox.Visible = false;
				break;
			}
			case 1:
			{
				MinMaxScoreMode = Mode.MiniMaxPruning;
				timeLimitHbox.Visible = false;
				break;
			}
			case 2:
			{
				MinMaxScoreMode = Mode.MiniMaxIterative;
				timeLimitHbox.Visible = true;
				break;
			}
		}
	} 
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (PlayerType.Selected == 0)
		{
			
		}
	}
}
