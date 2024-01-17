using Godot;
using System;

public partial class Camera : Camera2D
{
	Node2D target = null;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		target = GetNode<Node2D>("/root/Game/BeeKeeper"); 
		int limits = GetNode<Game>("/root/Game").MapSize / 2;
		LimitLeft = (int)-limits;
		LimitRight = (int)limits;
		LimitTop = (int)-limits;
		LimitBottom = (int)limits;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(target == null)
		{
			return;
		}
		Position = target.Position;
	}
}
