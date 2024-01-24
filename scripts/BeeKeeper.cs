using Godot;
using System;

public enum EInventoryItem
{
	None,
	Jar,
	BeeHive
}

public partial class BeeKeeper : CharacterBody2D
{
	[Export]
	public float Speed = 300.0f;
	[Export]
	public float Acceleration = 1200.0f;

	private int MapLimits = 0;

	[Export]
	public bool walking = false;
	[Export]
	public bool NearBeeHive = false;
	[Export]
	public float collectionCooldown = 1.0f;
	[Export]
	public float collectionTimer = 0.0f;
	[Export]
	public float collectionRate = 10.0f;
	[Export]
	public EInventoryItem CurrentItem = EInventoryItem.None;
	[Export]
	public int coins = 0;
	[Export]
	public float honeyAmount = 0.0f;
	[Export]
	public float honeyMax = 100.0f;
	[Export]
	public int beeHiveCount = 0;
	[Export]
	public BeeHive closeHive = null;

	[Signal]
	public delegate void ChangePlayerHoneyWithArgumentEventHandler(float amount, float max);
	[Signal]
	public delegate void ChangePlayerInventoryWithArgumentEventHandler(int item);
	[Signal]
	public delegate void ChangePlayerBeehiveCountWithArgumentEventHandler(int count);
	[Signal]
	public delegate void ChangePlayerCoinsWithArgumentEventHandler(int coins);

	enum EFacingDirection
	{
		Up,
		Down,
		Left,
		Right
	};
	
	EFacingDirection facingDirection = EFacingDirection.Down;
	AnimatedSprite2D beehiveSprite = null;
	AnimatedSprite2D jarSprite = null;
	PackedScene beehiveScene = null;
	AnimatedSprite2D playerSprite = null;

	public void SpawnBeehive() {
		BeeHive beehive = (BeeHive)beehiveScene.Instantiate();
		switch(facingDirection){
			case EFacingDirection.Up:
				beehive.Position = Position + new Vector2(0, -beehive.spawnDistanceFromPlayer);
				break;
			case EFacingDirection.Down:
				beehive.Position = Position + new Vector2(0, beehive.spawnDistanceFromPlayer);
				break;
			case EFacingDirection.Left:
				beehive.Position = Position + new Vector2(-beehive.spawnDistanceFromPlayer, 0);
				break;
			case EFacingDirection.Right:
				beehive.Position = Position + new Vector2(beehive.spawnDistanceFromPlayer, 0);
				break;
		}
		beehiveSprite.Visible = false;
		CurrentItem = EInventoryItem.None;
		beeHiveCount--;
		EmitSignal(SignalName.ChangePlayerInventoryWithArgument, 0);
		EmitSignal(SignalName.ChangePlayerBeehiveCountWithArgument, beeHiveCount);
		GetParent().AddChild(beehive);
	}

	public void ChangeInventoryItem(EInventoryItem item)
	{
		jarSprite.Visible = false;
		beehiveSprite.Visible = false;
		if(CurrentItem == item) {
			CurrentItem = EInventoryItem.None;
			EmitSignal(SignalName.ChangePlayerInventoryWithArgument, 0);
			return;
		}
		if(item == EInventoryItem.BeeHive){
			if(beeHiveCount <= 0){
				return;
			}
			beehiveSprite.Visible = true;
			EmitSignal(SignalName.ChangePlayerInventoryWithArgument, 1);
		}
		else if(item == EInventoryItem.Jar){
			jarSprite.Visible = true;
			EmitSignal(SignalName.ChangePlayerInventoryWithArgument, 2);
		}
		CurrentItem = item;
	}

	public void AddHoney(float amount){
		if(closeHive == null) {
			return;
		}
		if(closeHive.honey <= 0.0f){
			return;
		}else if(closeHive.honey < collectionRate){
			honeyAmount += closeHive.honey;
		}else {
			honeyAmount += amount;
		}
		closeHive.RemoveHoney(amount);
		if(honeyAmount > honeyMax){
			honeyAmount = honeyMax;
		}
		if (honeyAmount > honeyMax * 0.9f)
		{
			jarSprite.Frame = 4;
		}
		else if (honeyAmount > honeyMax * 0.75f)
		{
			jarSprite.Frame = 3;
		}
		else if (honeyAmount > honeyMax * 0.33f)
		{
			jarSprite.Frame = 2;
		}
		else if (honeyAmount > 0.0f)
		{
			jarSprite.Frame = 1;
		}
		else if (honeyAmount <= 0.0f)
		{
			jarSprite.Frame = 0;
		}
		EmitSignal(SignalName.ChangePlayerHoneyWithArgument, honeyAmount, honeyMax);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("Interact"))
		{
			if(CurrentItem == EInventoryItem.BeeHive){
				SpawnBeehive();
			}
			if(CurrentItem == EInventoryItem.Jar){
				if(collectionTimer <= 0.0f){
					collectionTimer = collectionCooldown;
					AddHoney(collectionRate);
				}
			}
		}
		if(@event.IsActionPressed("SelectItem1")){
			ChangeInventoryItem(EInventoryItem.BeeHive);
		} 
		else if(@event.IsActionPressed("SelectItem2")){
			ChangeInventoryItem(EInventoryItem.Jar);
		}
		else if(@event.IsActionPressed("MoveUp")){
			facingDirection = EFacingDirection.Up;
		} 
		else if(@event.IsActionPressed("MoveDown")){
			facingDirection = EFacingDirection.Down;
		} 
		else if(@event.IsActionPressed("MoveLeft")){
			facingDirection = EFacingDirection.Left;
		} 
		else if(@event.IsActionPressed("MoveRight")){
			facingDirection = EFacingDirection.Right;
		}
	}

	public override void _Ready() {
		MapLimits = Globals.MapSize / 2;
		beehiveSprite = GetNode<AnimatedSprite2D>("BeehiveSprite");
		jarSprite = GetNode<AnimatedSprite2D>("JarSprite");
		playerSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		GD.Print("MapLimits: " + MapLimits);
		beehiveScene = ResourceLoader.Load<PackedScene>("res://scenes/bee_hive.tscn");
	}

	public override void _Process(double delta)
	{
		if(collectionTimer > 0.0f){
			collectionTimer -= (float)delta;
		}
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
			velocity += direction * Acceleration * (float)delta;
			if (velocity.Length() > Speed)
			{
				velocity = direction * Speed;
			}
			if(!walking){
				walking = true;
				playerSprite.Play("walking");
			}
		}
		else
		{
			if(walking){
				walking = false;
				playerSprite.Play("idle");
			}
			velocity = velocity.MoveToward(Vector2.Zero, Acceleration * (float)delta);
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
