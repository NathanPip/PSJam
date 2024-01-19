using Godot;
using System;

public partial class BeeKeeper : CharacterBody2D
{
	[Export]
	public float Speed = 300.0f;

	private int MapLimits = 0;
	
	[Export]
	public bool holdingBeeHive = false;

	enum EFacingDirection
	{
		Up,
		Down,
		Left,
		Right
	};
	
	EFacingDirection facingDirection = EFacingDirection.Down;
	AnimatedSprite2D beehiveSprite = null;
	PackedScene beehiveScene = null;
	AnimatedSprite2D playerSprite = null;

	public void SpawnBeehive() {
		BeeHive beehive = (BeeHive)beehiveScene.Instantiate();
		switch(facingDirection){
			case EFacingDirection.Up:
				beehive.Position = Position + new Vector2(0, -beehive.distanceFromPlayer);
				break;
			case EFacingDirection.Down:
				beehive.Position = Position + new Vector2(0, beehive.distanceFromPlayer);
				break;
			case EFacingDirection.Left:
				beehive.Position = Position + new Vector2(-beehive.distanceFromPlayer, 0);
				break;
			case EFacingDirection.Right:
				beehive.Position = Position + new Vector2(beehive.distanceFromPlayer, 0);
				break;
		}
		holdingBeeHive = false;
		beehiveSprite.Visible = false;
		GetParent().AddChild(beehive);
	}

	public override void _Ready() {
		MapLimits = GetNode<Game>("/root/Game").MapSize / 2;
		beehiveSprite = GetNode<AnimatedSprite2D>("BeehiveSprite");
		playerSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		GD.Print("MapLimits: " + MapLimits);
		beehiveScene = ResourceLoader.Load<PackedScene>("res://scenes/bee_hive.tscn");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		Vector2 direction = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
		if (direction != Vector2.Zero)
		{
			if (direction.X < 0){
				playerSprite.FlipH = true;
			} else if (direction.X > 0){
				playerSprite.FlipH = false;
			}
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

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("Interact") && holdingBeeHive)
		{
			SpawnBeehive();
		}
		if(@event.IsActionPressed("MoveUp")){
			facingDirection = EFacingDirection.Up;
		} else if(@event.IsActionPressed("MoveDown")){
			facingDirection = EFacingDirection.Down;
		} else if(@event.IsActionPressed("MoveLeft")){
			facingDirection = EFacingDirection.Left;
		} else if(@event.IsActionPressed("MoveRight")){
			facingDirection = EFacingDirection.Right;
		}
	}

	public void OnInventoryChanged(string item)
	{
		holdingBeeHive = !holdingBeeHive;
		beehiveSprite.Visible = holdingBeeHive;
	}
}
