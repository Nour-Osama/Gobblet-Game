using Godot;
using System;

public partial class GobletScene : Node2D
{
	[Export] public Sprite2D Sprite2D;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void move(Position pos)
	{
		GameBoard g = GameManager.Instance.GameBoard;
		//check if position valid
		if (g.IsPositionValid(pos))
		{
			// move to new position
			Vector2I coords = new Vector2I(pos.x, pos.y);
			Vector2 vec2Pos = g.MapToLocal(coords);
			this.Set("position", vec2Pos);
		}
	}
}
