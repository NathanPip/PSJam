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
			ChangeHoneyRectHeight();
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
	private int _flowersBloomed = 0;
	public int flowersBloomed {
		get {
			return _flowersBloomed;
		}
		set {
			_flowersBloomed = value;
			if(_flowersBloomed >= sectorGridSize * sectorGridSize) {
				allFlowersBloomed = true;
			}
		}
	}
	public int bees = 0;
	public int spawnDistanceFromPlayer = 90;
	public float distanceFromPlauer = 0;
	public bool spawnedBees = false;
	public bool allFlowersBloomed = false;
	public List<FlowerPoint> flowerPoints = new List<FlowerPoint>();
	public List<Flower> flowers = new List<Flower>();
	public List<Flower> bloomedFlowers = new List<Flower>();
	PackedScene beeScene = ResourceLoader.Load<PackedScene>("res://scenes/bee.tscn");
	PackedScene flowerScene = ResourceLoader.Load<PackedScene>("res://scenes/flower.tscn");
	BeeKeeper player = null;
	ColorRect honeyMeter = null;
	Vector2 honeyMeterStartingPosition;
	AnimatedSprite2D beeHiveSprite = null;

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
			beeHiveSprite.Play("Full");
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

	public void SpawnFlowerPoint() {
		float sectorSize = flowerSpreadDistance / sectorGridSize;
		float halfSectorSize = sectorSize / 2;
		float halfGridSize = (float)sectorGridSize / 2;
		for(int i=-(int)halfGridSize; i<(int)Math.Ceiling(halfGridSize); i++) {
			for(int j=(int)-halfGridSize; j<(int)Math.Ceiling(halfGridSize); j++) {
				FlowerPoint flowerPoint = new FlowerPoint();
				Vector2 position = Position + new Vector2(j * sectorSize + halfSectorSize * (sectorGridSize % 2 == 0 ? 1 : 0) + GD.Randf() * sectorSize - halfSectorSize, 
						i * sectorSize + halfSectorSize * (sectorGridSize % 2 == 0 ? 1 : 0) + GD.Randf() * sectorSize - halfSectorSize);
				flowerPoint.position = position;
				flowerPoint.spawned = false;
				flowerPoints.Add(flowerPoint);
			}
		}
	}

	public void SpawnFlowerAtPoint(FlowerPoint point) {
		int index = flowerPoints.IndexOf(point);
		if(index >= 0) {
			SpawnFlowerAtPointIndex(index);
		}
	}

	public void SpawnFlowerAtPointIndex(int index) {
		FlowerPoint flowerPoint = flowerPoints[index];
        Flower flower = (Flower)flowerScene.Instantiate();
        flower.hive = this;
        flowers.Add((Flower)flower);
        flower.Position = flowerPoint.position;
        GetParent().AddChild(flower);
		flowerPoint.spawned = true;
		flowerPoints[index] = flowerPoint;
		flowerPoints.RemoveAt(index);
	}

	public void SpawnInitialSprouts() {
		int index1 = GD.RandRange(0, (int)flowerPoints.Count/3);;
		int index2 = GD.RandRange((int)flowerPoints.Count/3 + 1, (int)flowerPoints.Count/3 * 2 - 2);
		int index3 = GD.RandRange((int)flowerPoints.Count/3 * 2 + 1, flowerPoints.Count - 3);
		SpawnFlowerAtPointIndex(index1);
		SpawnFlowerAtPointIndex(index2);
		SpawnFlowerAtPointIndex(index3);
	}

	public void ChangeHoneyRectHeight() {
		honeyMeter.Position = honeyMeterStartingPosition with {Y = honeyMeterStartingPosition.Y - (honeyMeter.Size.Y * (honey / honeyLimit))};
	}

	public void AddHoney(int amount) {
		honey += amount;
	}

	public void RemoveHoney(float amount) 
	{
		honey -= amount;
		isFull = false;
		beeHiveSprite.Play("Idle");
		if(honey < 0) {
			honey = 0;
		}
	}

	public override void _Ready()
	{
		player = GetParent().GetNode<BeeKeeper>("BeeKeeper");
		honeyMeter = GetNode<ColorRect>("HoneyMeterMask/HoneyMeter");
		beeHiveSprite = GetNode<AnimatedSprite2D>("BeeHiveSprite");
		honeyMeterStartingPosition = honeyMeter.Position;
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
