using Godot;
using System;
using System.Collections.Generic;

public struct FlowerPoint {
	public Vector2 position;
	public bool spawned;
}

public partial class BeeHive : Node2D
{
	[Export]
	public int flowerSpreadDistance = 300;
	[Export]
	public int startingBees = 10;
	[Export]
	public int beeMoveRange = 100;
	[Export]
	public int honey = 0;
	[Export]
	public int flowerSpawnAmount = 40;
	[Export]
	public float initialFlowerSproutSpawnChance = .1f;
	public int bees = 0;
	public int distanceFromPlayer = 90;
	public bool spawnedBees = false;
	public int flowersBloomed = 0;
	public bool isFull = false;
	public List<FlowerPoint> flowerPoints = new List<FlowerPoint>();
	public List<Flower> flowers = new List<Flower>();
	PackedScene beeScene = ResourceLoader.Load<PackedScene>("res://scenes/bee.tscn");
	PackedScene flowerScene = ResourceLoader.Load<PackedScene>("res://scenes/flower.tscn");

	public void SpawnBees() {
		for(int i=0; i< startingBees; i++) {
			Bee bee = (Bee)beeScene.Instantiate();
			bee.Position = GlobalPosition;
			bee.hive = this;
			bees++;
			GetParent().AddChild(bee);
		}
	}

	public void SpawnFlowerPoint() {
		for(int i=0; i < flowerSpawnAmount; i++) {
			FlowerPoint flowerPoint = new FlowerPoint();
			flowerPoint.position = new Vector2(GD.RandRange(-flowerSpreadDistance, flowerSpreadDistance), GD.RandRange(-flowerSpreadDistance, flowerSpreadDistance));
			flowerPoint.spawned = false;
			flowerPoints.Add(flowerPoint);
		}
	}

	public void SpawnInitialSprouts() {
		foreach(FlowerPoint point in flowerPoints) {
			if(point.spawned) {
				continue;
			}
			float chance = GD.Randf();
			if(chance > initialFlowerSproutSpawnChance) {
				continue;
			}
			Flower flower = (Flower)flowerScene.Instantiate();
			flowers.Add((Flower)flower);
			flower.Position = Position + point.position;
			GetParent().AddChild(flower);
		}
	}

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
			SpawnFlowerPoint();
			SpawnInitialSprouts();
			spawnedBees = true;
		}
	}
}
