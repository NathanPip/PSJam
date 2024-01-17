using Godot;
using System;

public partial class BeehivePlayerSprite : Sprite2D
{

	Vector2 startingPosition;

	public override void _Input(InputEvent @event)
	{
		if(@event.IsActionPressed("MoveRight")){
			Position = startingPosition;
		}
		if (@event.IsActionPressed("MoveLeft"))
		{
			Position = Position with { X = -startingPosition.X }; 
		}
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		startingPosition = Position;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
