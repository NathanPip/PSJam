using Godot;
using System;

public partial class BeeHive : Node2D
{
	public int bees = 0;
	public int distanceFromPlayer = 90;
	public int beeMoveRange = 100;
	public bool spawnedBees = false;
	public int honey = 0;
	public int flowersBloomed = 0;
	public bool isFull = false;
	PackedScene beeScene = ResourceLoader.Load<PackedScene>("res://scenes/bee.tscn");
	
	public void SpawnBees() {
		for(int i=0; i< 10; i++) {
			Bee bee = (Bee)beeScene.Instantiate();
			bee.Position = GlobalPosition;
			bee.hive = this;
			bees++;
			GetParent().AddChild(bee);
		}
	}

	public void SpawnFlowerPoint() {}

	public void AddHoney(int amount) {
		honey += amount;
	}

	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!spawnedBees){
			SpawnBees();
			spawnedBees = true;
		}
	}
}
