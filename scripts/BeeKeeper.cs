using Godot;
using System;

public enum EInventoryItem
{
	None,
	Jar,
	BeeHive,
	WateringCan
}

public partial class BeeKeeper : CharacterBody2D
{
	[Export]
	public float Speed = 300.0f;
	[Export]
	public float Acceleration = 1200.0f;
	[Export]
	public bool walking = false;
	[Export]
	public bool NearBeeHive = false;
	[Export]
	public float collectionRate = 10.0f;
	[Export]
	public EInventoryItem CurrentItem = EInventoryItem.None;
	[Export]
	public int coins = 0;
	[Export]
	public float waterMax = 100.0f;
	float _waterAmount = 0.0f;
	[Export]
	public float waterAmount {
		get {
			return _waterAmount;
		} set {
			_waterAmount = value;
			if(_waterAmount > waterMax){
				_waterAmount = waterMax;
			}
			if(_waterAmount < 0.0f){
				_waterAmount = 0.0f;
			}
			EmitSignal(SignalName.ChangePlayerWaterAmountWithArgument, _waterAmount, waterMax);
		}
	}
	float _honeyAmount = 0.0f;
	[Export]
	public float honeyAmount {
		get {
			return _honeyAmount;
		} set {
			_honeyAmount = value;
			if(_honeyAmount > honeyMax){
				_honeyAmount = honeyMax;
			}
			if(_honeyAmount < 0.0f){
				_honeyAmount = 0.0f;
			}
			EmitSignal(SignalName.ChangePlayerHoneyWithArgument, _honeyAmount, honeyMax);
		}
	}
	[Export]
	public float honeyMax = 100.0f;
	int _beeHiveCount = 0;
	[Export]
	public int beeHiveCount {
		get {
			return _beeHiveCount;
		} set {
			_beeHiveCount = value;
			EmitSignal(SignalName.ChangePlayerBeehiveCountWithArgument, _beeHiveCount);
		}
	}
	[Export]
	public BeeHive closeHive = null;

	public Flower closeFlower = null;

	public bool canCollectWater = false;

	public bool canInteractWithShop = false;

	public bool collectingHoney = false;

	public bool collectingWater = false;

	private int MapLimits = 0;

	public bool inDialogue = false;

	[Signal]
	public delegate void ChangePlayerHoneyWithArgumentEventHandler(float amount, float max);
	[Signal]
	public delegate void ChangePlayerInventoryWithArgumentEventHandler(int item);
	[Signal]
	public delegate void ChangePlayerBeehiveCountWithArgumentEventHandler(int count);
	[Signal]
	public delegate void ChangePlayerWaterAmountWithArgumentEventHandler(float water, float max);
	[Signal]
	public delegate void PlayerInteractedWithShopEventHandler();
	[Signal]
	public delegate void ShowScreenHintWithArgumentEventHandler(string text);
	[Signal]
	public delegate void HideScreenHintEventHandler();

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

	public void WaterCloseFlower() {
		if(waterAmount <= 0.0f){
			return;
		}
		if(waterAmount <= 10.0f){
			waterAmount = 0.0f;
		} else {
			waterAmount -= 10.0f;
		}
		if(closeFlower == null) {
			return;
		}
		closeFlower.WaterFlower();
		closeFlower = null;
	}

	public void CollectWater(float amount) {
		if(waterAmount >= 100.0f){
			return;
		}
		if(waterAmount + amount >= 100.0f){
			waterAmount = 100.0f;
		} else {
			waterAmount += amount;
		}
		if(playerSprite.Animation != "collecting_water") {
			playerSprite.Play("collecting_water");
		}
	}

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
		Globals.totalBeehivesPlaced++;
		beehiveSprite.Visible = false;
		CurrentItem = EInventoryItem.None;
		beeHiveCount--;
		EmitSignal(SignalName.ChangePlayerInventoryWithArgument, 0);
		EmitSignal(SignalName.HideScreenHint);
		EmitSignal(SignalName.ChangePlayerBeehiveCountWithArgument, beeHiveCount);
		GetParent().AddChild(beehive);
	}

	public void ChangeInventoryItem(EInventoryItem item)
	{
		beehiveSprite.Visible = false;
		collectingWater = false;
		collectingHoney = false;
		if(CurrentItem == item) {
			CurrentItem = EInventoryItem.None;
			if(!walking){
				playerSprite.Play("idle");
			} else {
				playerSprite.Play("walking");
			}
			EmitSignal(SignalName.ChangePlayerInventoryWithArgument, 0);
			EmitSignal(SignalName.HideScreenHint);
			return;
		}
		if(item == EInventoryItem.BeeHive){
			if(beeHiveCount <= 0){
				return;
			}
			beehiveSprite.Visible = true;
			EmitSignal(SignalName.ShowScreenHintWithArgument, "Press Space to place beehive");
			EmitSignal(SignalName.ChangePlayerInventoryWithArgument, 1);
		}
		else if(item == EInventoryItem.Jar){
			if(!walking){
				playerSprite.Play("idle_with_honey");
			} else {
				playerSprite.Play("walking_with_honey");
			}
			if(closeHive != null){
				EmitSignal(SignalName.ShowScreenHintWithArgument, "Hold Space to collect honey");
			}
			EmitSignal(SignalName.ChangePlayerInventoryWithArgument, 2);
		}else if(item == EInventoryItem.WateringCan){
			if(!walking){
				playerSprite.Play("idle_with_water");
			} else {
				playerSprite.Play("walking_with_water");
			}
			if(canCollectWater){
				EmitSignal(SignalName.ShowScreenHintWithArgument, "Hold Space to collect water");
			}
			if(closeFlower != null && waterAmount > 0){
			   EmitSignal(SignalName.ShowScreenHintWithArgument, "Press Space to water flower");
			}
			EmitSignal(SignalName.ChangePlayerInventoryWithArgument, 3);
		}
		CurrentItem = item;
	}

	public void AddHoney(float amount){
		if(honeyAmount >= honeyMax){
			return;
		}
		float previousHoneyAmount = honeyAmount;
		if(closeHive == null) {
			return;
		}
		if(closeHive.honey <= 0.0f){
			return;
		}else if(closeHive.honey < amount){
			honeyAmount += closeHive.honey;
		}else {
			honeyAmount += amount;
		}
		if(playerSprite.Animation != "collecting_honey"){
			playerSprite.Play("collecting_honey");
		}
		Globals.totalHoneyCollected += honeyAmount - previousHoneyAmount;
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
	}

	public override void _Input(InputEvent @event)
	{
        if (inDialogue)
        {
            return;
        }
		if (@event.IsActionPressed("Interact"))
		{
			if(CurrentItem == EInventoryItem.BeeHive){
				SpawnBeehive();
			}
			if(CurrentItem == EInventoryItem.WateringCan){
				if(waterAmount > 0 && closeFlower != null){
					playerSprite.Play("watering");
					WaterCloseFlower();
				} else if(canCollectWater){
					collectingWater = true;
				}
			}
			if(CurrentItem == EInventoryItem.Jar){
				if(closeHive != null){
					collectingHoney = true;
				}
			}
			if(canInteractWithShop) {
				GD.Print("opened shop");
				EmitSignal(SignalName.PlayerInteractedWithShop);
			}
		}
		if(@event.IsActionReleased("Interact")){
			if(CurrentItem == EInventoryItem.Jar){
				collectingHoney = false;
				playerSprite.Play("idle_with_honey");
			}
			if(CurrentItem == EInventoryItem.WateringCan){
				collectingWater = false;
				playerSprite.Play("idle_with_water");
			}
		}
		if(@event.IsActionPressed("SelectItem1")){
			ChangeInventoryItem(EInventoryItem.BeeHive);
		} 
		else if(@event.IsActionPressed("SelectItem2")){
			ChangeInventoryItem(EInventoryItem.Jar);
		}
		else if(@event.IsActionPressed("SelectItem3")){
			ChangeInventoryItem(EInventoryItem.WateringCan);
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
		if(playerSprite.Animation == "watering" && playerSprite.Frame == 3){
			playerSprite.Play("idle");
		}
		if(collectingHoney){
			AddHoney(collectionRate * (float)delta);
		}
		if(collectingWater){
			CollectWater(collectionRate * (float)delta);
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
				switch(CurrentItem){
					case EInventoryItem.Jar:
						playerSprite.Play("walking_with_honey");
						break;
					case EInventoryItem.WateringCan:
						playerSprite.Play("walking_with_water");
						break;
					default:
						playerSprite.Play("walking");
						break;
				}
			}
		}
		else
		{
			if(walking){
				walking = false;
				switch(CurrentItem){
					case EInventoryItem.Jar:
						playerSprite.Play("idle_with_honey");
						break;
					case EInventoryItem.WateringCan:
						playerSprite.Play("idle_with_water");
						break;
					default:
						playerSprite.Play("idle");
						break;
				}
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

	public void OnFlowerAreaEntered(Area2D area) {
		Node2D node = area.GetParent<Node2D>();
		GD.Print("areaEntered");
		if(node is Flower){
			Flower flower = (Flower)node;
			if(flower.watered){
				return;
			}
			if(closeFlower == null){
				closeFlower = flower;
			} else {
			 		if(node.Position.DistanceTo(Position) < closeFlower.Position.DistanceTo(Position)){
					closeFlower = flower;
				}
			}
			if(CurrentItem == EInventoryItem.WateringCan) {
				EmitSignal(SignalName.ShowScreenHintWithArgument, "Press Space to water flower");
			}
		}
		if(node is Well) {
			canCollectWater = true;
			if(CurrentItem == EInventoryItem.WateringCan && waterAmount > 0) {
				EmitSignal(SignalName.ShowScreenHintWithArgument, "Hold Space to collect water");
			}
		}
	}

	public void OnFlowerAreaExited(Area2D area) {
		Node2D node = area.GetParent<Node2D>();
		if(node is Flower){
			Flower flower = (Flower)node;
			if(closeFlower == flower){
				closeFlower = null;
			}
			if(CurrentItem == EInventoryItem.WateringCan) {
				EmitSignal(SignalName.HideScreenHint);
			}
		}
		if(node is Well) {
			canCollectWater = false;
			if(CurrentItem == EInventoryItem.WateringCan) {
				EmitSignal(SignalName.HideScreenHint);
			}
		}
	}


}
