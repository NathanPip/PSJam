using Godot;
using System;

public partial class Game : Node2D
{
	[Export]
	public int MapSize = 2000;
	
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
