using Godot;
using System;

public partial class AbstractGobbletScene : Sprite2D
{
	[Export] private Area2D Area2D;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Area2D.InputEvent += input_event;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	void input_event(Node node, InputEvent e,long shape)
	{
		if (e is InputEventMouseButton &&
		    ((InputEventMouseButton)e).ButtonIndex == MouseButton.Left&&
		    ((InputEventMouseButton)e).Pressed
		    )
		{
			GameBoard g = GameManager.Instance.GameBoard;
			Position pos = g.getPos(g.MapToLocal(g.LocalToMap(GetLocalMousePosition())));
			//GameManager.Instance.Gobblet_clicked(pos.GetGobblet());
		}
			
	}
}
