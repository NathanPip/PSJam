using Godot;
using System;

public partial class BeehivePlayerSprite : AnimatedSprite2D
{

	Vector2 startingPosition;

	public override void _Input(InputEvent @event)
	{
		if(@event.IsActionPressed("MoveRight")){
			Position = startingPosition;
		}
		if (@event.IsActionPressed("MoveLeft"))
		{
			Position = Position with { X = -startingPosition.X, Y = 0 }; 
		}
		if(@event.IsActionPressed("MoveUp")){
			Position = Position with { Y = -startingPosition.X, X = 0 }; 
		}
		if(@event.IsActionPressed("MoveDown")){
			Position = Position with { Y = startingPosition.X, X = 0 };
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
