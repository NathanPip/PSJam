using Godot;
using System;

public partial class Camera : Camera2D
{
	Node2D target = null;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		target = GetNode<Node2D>("/root/Game/BeeKeeper"); 
		int limits = Globals.MapSize / 2;
		LimitLeft = (int)-limits*2;
		LimitRight = (int)limits*2;
		LimitTop = (int)-limits*2;
		LimitBottom = (int)limits*2;
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
