using Godot;
using System;

public partial class BeeKeeper : CharacterBody2D
{
	[Export]
	public float Speed = 300.0f;

	private int MapLimits = 0;

	public override void _Ready() {
		MapLimits = GetNode<Game>("/root/Game").MapSize / 2;
		GD.Print("MapLimits: " + MapLimits);
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		Vector2 direction = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
		if (direction != Vector2.Zero)
		{
			velocity = direction * Speed;
		}
		else
		{
			velocity = velocity.MoveToward(Vector2.Zero, Speed);
		}
		Vector2 nextPosition = Position + velocity * (float)delta;
		if (nextPosition.X < -MapLimits || nextPosition.X > MapLimits)
		{
			velocity = velocity with { X = 0 };
		}
		if (nextPosition.Y < -MapLimits || nextPosition.Y > MapLimits)
		{
			velocity = velocity with { Y = 0 };
		}
		Velocity = velocity;
		MoveAndSlide();
	}
}
