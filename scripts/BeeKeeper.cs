using Godot;
using System;
using System.Collections.Generic;

public enum EInventoryItem
{
	None,
	Jar,
	BeeHive,
	WateringCan
}

public enum EPlayerAction
{
	Idle,
	Walking,
	Collecting,
	Watering,
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
	public float wateringRate = 5.0f;
	private EInventoryItem _currentItem = EInventoryItem.None;
	[Export]
	public EInventoryItem CurrentItem {
		get {
			return _currentItem;
		} set {
			_currentItem = value;
			SwitchAnimation();
			UpdateInteraction();
		}
	}
	private EPlayerAction _currentAction = EPlayerAction.Idle;
	[Export]
	public EPlayerAction CurrentAction {
		get {
			return _currentAction;
		} set {
			_currentAction = value;
			SwitchAnimation();
		}
	}
	private Node2D _currentInteractable = null;
	[Export]
	public Node2D CurrentInteractable {
		get {
			return _currentInteractable;
		} set {
			_currentInteractable = value;
		}
	} 
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
	public bool canCollectWater = false;

	public bool canInteractWithShop = false;

	public bool canWaterFlower = false;

	public bool canCollectHoney = false;

	public bool canMove = true;

	public bool collectingHoney = false;

	public bool collectingWater = false;

	public bool watering = false;

	public bool moving = false;

	private int MapLimits = 0;

	public bool inDialogue = false;


	public List<Node2D> nearbyInteractables = new List<Node2D>();

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

	public void SwitchAnimation() 
	{
		string animationString = "";
		animationString += CurrentAction.ToString();
		string itemString = "";
		if(CurrentItem != EInventoryItem.None && CurrentItem != EInventoryItem.BeeHive){
			itemString += CurrentItem.ToString();
		}
		animationString += itemString;
		if(playerSprite.Animation != animationString){
			playerSprite.Play(animationString);
		}
	}

	public void SortInteractablesByDistance() {
		if(nearbyInteractables.Count == 0 || nearbyInteractables.Count == 1){
			return;
		}
		nearbyInteractables.Sort((x, y) => Position.DistanceTo(x.Position).CompareTo(Position.DistanceTo(y.Position)));
	}

	public void UpdateInteraction() {
		if(nearbyInteractables.Count == 0){
			CurrentInteractable = null;
			EmitSignal(SignalName.HideScreenHint);
			return;
		}
		if(CurrentInteractable != null) {
			GD.Print(CurrentInteractable.Name);
		} 		
		foreach(Node2D interactable in nearbyInteractables) {
			if(CurrentItem == EInventoryItem.WateringCan) {
				if(interactable is Flower){
					if(waterAmount <= 0.0f){
						EmitSignal(SignalName.ShowScreenHintWithArgument, "You don't have any water");
						return;
					}
					CurrentInteractable = interactable;
					EmitSignal(SignalName.ShowScreenHintWithArgument, "Press Space to water the flower");
					return;
				}
				if(interactable is Well){
					EmitSignal(SignalName.ShowScreenHintWithArgument, "Press Space to collect water");
					CurrentInteractable = interactable;
					return;
				}
			} else if(CurrentItem == EInventoryItem.Jar) {
				if(interactable is BeeHive && honeyAmount < honeyMax){
					BeeHive beehive = (BeeHive)interactable;
					if(beehive.honey <= 0.0f){
						EmitSignal(SignalName.ShowScreenHintWithArgument, "The beehive is empty");
						return;
					}
					EmitSignal(SignalName.ShowScreenHintWithArgument, "Press Space to collect honey");
					CurrentInteractable = interactable;
					return;
				}
			}
			if(interactable is Shop){
				EmitSignal(SignalName.ShowScreenHintWithArgument, "Press Space to talk with the shopkeeper");
				CurrentInteractable = interactable;
				return;
			}
		}
		EmitSignal(SignalName.HideScreenHint);
		CurrentInteractable = null;
	}

	public void AddNearbyInteractable(Node2D interactable) {
		if(nearbyInteractables.Contains(interactable)){
			return;
		}
		nearbyInteractables.Add(interactable);
		UpdateInteraction();
	}

	public void RemoveNearbyInteractable(Node2D interactable) {
		nearbyInteractables.Remove(interactable);
		UpdateInteraction();
	} 

	public void Interact() {
		if(CurrentItem == EInventoryItem.WateringCan) {
			if(moving){
				return;
			}
			if(CurrentInteractable is Well){
				CurrentAction = EPlayerAction.Collecting;
				collectingWater = true;
			} else {
				if(waterAmount <= 0.0f){
					return;
				}
				CurrentAction = EPlayerAction.Watering;
				watering = true;
			}
		}
		if(CurrentItem == EInventoryItem.Jar) {
			if(CurrentInteractable is BeeHive){
				CurrentAction = EPlayerAction.Collecting;
				collectingHoney = true;
			}
		}
		if(CurrentItem == EInventoryItem.BeeHive){
			SpawnBeehive();
		}
		if(CurrentInteractable is Shop){
			CurrentAction = EPlayerAction.Idle;
			EmitSignal(SignalName.PlayerInteractedWithShop);
		}
	}

	public void StopInteracting() {
		if(CurrentAction != EPlayerAction.Walking){
			CurrentAction = EPlayerAction.Idle;
		}
		watering = false;
		collectingWater = false;
		collectingHoney = false;
	}

	public void Water(float amount){
		if(moving){
			watering = false;
			return;
		}
		waterAmount -= amount;
		if(waterAmount <= 0.0f){
			StopInteracting();
		}
		if(CurrentInteractable is Flower){
			Flower flower = (Flower)CurrentInteractable;
			flower.AddWater(amount);
		}
	}

	public void CollectWater(float amount) {
		if(waterAmount >= 100.0f){
			StopInteracting();
			return;
		}
		if(waterAmount + amount >= 100.0f){
			waterAmount = 100.0f;
		} else {
			waterAmount += amount;
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
		if(beehive.Position.X < -MapLimits){
			beehive.Position += new Vector2(-MapLimits-beehive.Position.X, 0);
		}
		if(beehive.Position.Y < -MapLimits){
			beehive.Position += new Vector2(0, -MapLimits-beehive.Position.Y);
		}
		if(beehive.Position.X > MapLimits){
			beehive.Position += new Vector2(MapLimits-beehive.Position.X, 0);
		}
		if(beehive.Position.Y > MapLimits){
			beehive.Position += new Vector2(0, MapLimits-beehive.Position.Y);
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

	public void AddHoney(float amount){
		if(honeyAmount >= honeyMax){
			StopInteracting();
			return;
		}
		float previousHoneyAmount = honeyAmount;
		BeeHive closeHive = (BeeHive)CurrentInteractable;
		if(closeHive == null) {
			return;
		}
		if(closeHive.honey <= 0.0f){
			return;
		}else if(closeHive.honey < amount){
			honeyAmount += closeHive.honey;
			StopInteracting();
		}else {
			honeyAmount += amount;
		}
		Globals.totalHoneyCollected += honeyAmount - previousHoneyAmount;
		closeHive.RemoveHoney(amount);
		if(honeyAmount > honeyMax){
			honeyAmount = honeyMax;
			StopInteracting();
		}
	}

	public void ChangeInventoryItem(EInventoryItem item)
	{
		beehiveSprite.Visible = false;
		collectingWater = false;
		collectingHoney = false;
		if(CurrentItem == item) {
			CurrentItem = EInventoryItem.None;
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
			EmitSignal(SignalName.ChangePlayerInventoryWithArgument, 2);
		}else if(item == EInventoryItem.WateringCan){
			EmitSignal(SignalName.ChangePlayerInventoryWithArgument, 3);
		}
		CurrentItem = item;
	}


	public override void _Input(InputEvent @event)
	{
        if (inDialogue)
        {
            return;
        }
		if (@event.IsActionPressed("Interact"))
		{
			Interact();
		}
		if(@event.IsActionReleased("Interact")){
			StopInteracting();
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
		playerSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		GD.Print("MapLimits: " + MapLimits);
		beehiveScene = ResourceLoader.Load<PackedScene>("res://scenes/bee_hive.tscn");
	}

	public override void _Process(double delta)
	{
		if(collectingHoney){
			AddHoney(collectionRate * (float)delta);
		}
		if(collectingWater){
			CollectWater(collectionRate * (float)delta);
		}
		if(watering){
			Water(wateringRate * (float)delta);
		}
		if(!canMove) {
			if(playerSprite.Animation != "watering" || playerSprite.Animation == "watering" && playerSprite.Frame == 4){
				canMove = true;
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		Vector2 direction = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
		if(velocity.Length() > 0){
			UpdateInteraction();
			SortInteractablesByDistance();
		}  else {
		}		
		if (direction != Vector2.Zero)
		{
			if(!canMove){
				return;
			}
			moving = true;
			CurrentAction = EPlayerAction.Walking;
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
            walking = true;
			collectingHoney = false;
			collectingWater = false;
		}
		else
		{
			moving = false;
			if(CurrentAction == EPlayerAction.Walking){
				CurrentAction = EPlayerAction.Idle;
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
		if(node is Flower){
			Flower flower = (Flower)node;
			AddNearbyInteractable(flower);
		}
		if(node is Well) {
			Well well = (Well)node;
			AddNearbyInteractable(well);
		}
		if(node is BeeHive){
			BeeHive hive = (BeeHive)node;
			AddNearbyInteractable(hive);
		}
		if(node is Shop){
			Shop shop = (Shop)node;
			AddNearbyInteractable(shop);
		}
	}

	public void OnFlowerAreaExited(Area2D area) {
		Node2D node = area.GetParent<Node2D>();
		if(node is Flower){
			Flower flower = (Flower)node;
			RemoveNearbyInteractable(flower);
		}
		if(node is Well) {
			Well well = (Well)node;
			RemoveNearbyInteractable(well);
		}
		if(node is BeeHive){
			BeeHive hive = (BeeHive)node;
			RemoveNearbyInteractable(hive);
		}
		if(node is Shop){
			Shop shop = (Shop)node;
			RemoveNearbyInteractable(shop);
		}
	}


}
