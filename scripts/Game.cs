using Godot;
using System;

public struct Inventory
{
	int money;
	int honey;
	int hives;
}

public partial class Game : Node2D
{

	[Export]
	public int MapSize = 2000;

	[Signal]
	public delegate void InventoryChangedEventHandler();

	Inventory inventory = new Inventory();

    public override void _Input(InputEvent @event)
    {
		if(@event.IsActionPressed("SelectItem1")){
			EmitSignal(SignalName.InventoryChanged);
		}
    }
    // Called when the node enters the scene tree for the first time.
    
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PackedScene beehiveScene = ResourceLoader.Load<PackedScene>("res://scenes/bee_hive.tscn");
		// STRESS TEST
	// 	for(int i=0; i< 100; i++) {
	// 	 	BeeHive beehive = (BeeHive)beehiveScene.Instantiate();
	// 		beehive.Position = new Vector2(GD.RandRange(-MapSize/2, MapSize/2), GD.RandRange(-MapSize/2, MapSize/2));
	// 		AddChild(beehive);
	// 	}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
