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
	private float _honey = 0;
	[Export]
	public float honey {
		get {
			return _honey;
		}
		set {
			_honey = value;
			EmitSignal(SignalName.AddHoneyWithArgument, _honey);
		}
	}
	[Export]
	public int flowerSpawnAmount = 40;
	[Export]
	public float initialFlowerSproutSpawnChance = .1f;
	[Export]
	public float sectorGridSize = 4;
	[Export]
	public bool isFull = false;
	[Export]
	public float honeyLimit = 100;
	public int bees = 0;
	public int spawnDistanceFromPlayer = 90;
	public float distanceFromPlauer = 0;
	public bool spawnedBees = false;
	public int flowersBloomed = 0;
	public List<FlowerPoint> flowerPoints = new List<FlowerPoint>();
	public List<Flower> flowers = new List<Flower>();
	PackedScene beeScene = ResourceLoader.Load<PackedScene>("res://scenes/bee.tscn");
	PackedScene flowerScene = ResourceLoader.Load<PackedScene>("res://scenes/flower.tscn");
	BeeKeeper player = null;

	[Signal]
	public delegate void AddHoneyWithArgumentEventHandler(float honeyAmount);

	public void SpawnBees() {
		for(int i=0; i< startingBees; i++) {
			Bee bee = (Bee)beeScene.Instantiate();
			bee.Position = GlobalPosition;
			bee.hive = this;
			bees++;
			GetParent().AddChild(bee);
		}
	}

	public void StoreHoney(float honeyAmount) {
		honey += honeyAmount;
		if(honey >= honeyLimit){
			honey = honeyLimit;
			isFull = true;
		}
	}

	public void AddFlower(Vector2 position) {
		Flower flower = (Flower)flowerScene.Instantiate();
		flowers.Add((Flower)flower);
		flower.Position = position;
		GetParent().AddChild(flower);
	}

	public void SpawnFlower(int index) {
	 	FlowerPoint flowerPoint = flowerPoints[index];
		Flower flower = (Flower)flowerScene.Instantiate();
		flowers.Add((Flower)flower);
		flower.Position = flowerPoint.position;
		GetParent().AddChild(flower);
		flowerPoint.spawned = true;
		flowerPoints[index] = flowerPoint;
	}

	public void SpawnFlowerPoint() {
		float sectorSize = flowerSpreadDistance / sectorGridSize;
		float halfSectorSize = sectorSize / 2;
		float halfGridSize = (float)sectorGridSize / 2;
		for(int i=-(int)halfGridSize; i<(int)Math.Ceiling(halfGridSize); i++) {
			for(int j=(int)-halfGridSize; j<(int)Math.Ceiling(halfGridSize); j++) {
				FlowerPoint flowerPoint = new FlowerPoint();
				GD.Print(sectorSize * sectorGridSize % 2);
				Vector2 position = Position + new Vector2(j * sectorSize + halfSectorSize * (sectorGridSize % 2 == 0 ? 1 : 0) + GD.Randf() * sectorSize - halfSectorSize, 
						i * sectorSize + halfSectorSize * (sectorGridSize % 2 == 0 ? 1 : 0) + GD.Randf() * sectorSize - halfSectorSize);
				flowerPoint.position = position;
				flowerPoint.spawned = false;
				flowerPoints.Add(flowerPoint);
			}
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
			flower.Position = point.position;
			GetParent().AddChild(flower);
		}
	}

	public void AddHoney(int amount) {
		honey += amount;
	}

	public void RemoveHoney(float amount) 
	{
		honey -= amount;
		if(honey < 0) {
			honey = 0;
		}
	}

	public override void _Ready()
	{
		player = GetParent().GetNode<BeeKeeper>("BeeKeeper");
		distanceFromPlauer = GlobalPosition.DistanceTo(player.GlobalPosition);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		distanceFromPlauer = GlobalPosition.DistanceTo(player.GlobalPosition);
		if(distanceFromPlauer < 50) {
			if(player.closeHive == null) {
				player.closeHive = this;
			} else if (player.closeHive.distanceFromPlauer > distanceFromPlauer) {
				player.closeHive = this;
			}
		} else {
			if(player.closeHive == this){
				player.closeHive = null;
			}
		}
		if(!spawnedBees){
			SpawnBees();
			SpawnFlowerPoint();
			SpawnInitialSprouts();
			spawnedBees = true;
		}
	}
}
