using Godot;

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
			Position = Position with { X = -startingPosition.X, Y = -9 }; 
		}
		if(@event.IsActionPressed("MoveUp")){
			Position = Position with { Y = -startingPosition.X - 9, X = 0 }; 
		}
		if(@event.IsActionPressed("MoveDown")){
			Position = Position with { Y = startingPosition.X - 9, X = 0 };
		}
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		startingPosition = Position with { Y = -9 };
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
