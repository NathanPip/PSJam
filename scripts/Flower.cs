using Godot;
using System;
using System.Collections.Generic;

public partial class Flower : Node2D
{
	[Export]
	public float bloomSpeed = 5;
	[Export]
	public bool hasSprouted = false;
	[Export]
	public float minSproutTime = 3;
	[Export]
	public float maxSproutTime = 10;
	[Export]
	public int spawnTilesPerBloomAmount = 5;
	[Export]
	public float spawnFlowerTilesChance = .6f;
	[Export]
	public float spawnGrassTilesChance = .3f;
	[Export]
	public int chancesToSpawnTile = 5;
	[Export]
	public int chancesToSpawnTileOnBloom = 10;
	public float nextBloom;
	public float sproutTimer = 0;
	public float sproutTime = 0;
	public bool isBloomed = false;
	public float bloomAmount = 1;
	public BeeHive hive = null;
	public Meadow meadow = null;
	public List<Vector2I> neighboringTiles = new List<Vector2I>();
	public GpuParticles2D bloomParticles = null;
	public PackedScene bloomFlyer = ResourceLoader.Load<PackedScene>("res://scenes/bloom_flyer.tscn");

	Texture2D MedusaFlower = (Texture2D)ResourceLoader.Load("res://assets/sprites/Medusaflower.png");
	Texture2D BlackEyeSusanFlower = (Texture2D)ResourceLoader.Load("res://assets/sprites/blackeyesusan.png");
	Texture2D BlueFlower = (Texture2D)ResourceLoader.Load("res://assets/sprites/blueflower.png");
	Texture2D MouthFlower = (Texture2D)ResourceLoader.Load("res://assets/sprites/mouthflower.png");
	Texture2D PinkFlower = (Texture2D)ResourceLoader.Load("res://assets/sprites/pink_flower.png");

	public List<Texture2D> textures = new List<Texture2D>() {
	 	(Texture2D)ResourceLoader.Load("res://assets/sprites/Medusaflower.png"),
		(Texture2D)ResourceLoader.Load("res://assets/sprites/blackeyesusan.png"),
        (Texture2D)ResourceLoader.Load("res://assets/sprites/blueflower.png"),
        (Texture2D)ResourceLoader.Load("res://assets/sprites/mouthflower.png"),
		(Texture2D)ResourceLoader.Load("res://assets/sprites/pink_flower.png")
    };

	public void SpawnTiles(int chances) {
        for (int i = 0; i < chances; i++)
        {
            float chance = GD.Randf();
            if (chance <= spawnGrassTilesChance)
            {
                Vector2I tile = neighboringTiles[(int)Mathf.Floor(GD.RandRange(0, neighboringTiles.Count - 1))];
                TileData cellData = meadow.GetCellTileData(1, tile);
                if (cellData != null) { continue; }
                meadow.SetCell(1, tile, 1, new Vector2I(GD.RandRange(0, 7), 0));
            }
            else if (chance <= spawnFlowerTilesChance)
            {
                Vector2I tile = neighboringTiles[(int)Mathf.Floor(GD.RandRange(0, neighboringTiles.Count - 1))];
                TileData cellData = meadow.GetCellTileData(1, tile);
                if (cellData != null) { continue; }
                meadow.SetCell(1, tile, 1, new Vector2I(GD.RandRange(0, 7), GD.RandRange(2, 7)));
            }
        }
	}

	public void BloomFlower() {
        // bloomParticles.Emitting = true;
		SpawnTiles(chancesToSpawnTileOnBloom);
        bloomAmount = 100;
        isBloomed = true;
        int index = GD.RandRange(0, hive.flowerPoints.Count - 1);
		BloomFlyer flyer = (BloomFlyer)bloomFlyer.Instantiate();
		flyer.hive = hive;
		flyer.flowerPoint = hive.flowerPoints[index];
		flyer.Position = GlobalPosition;
		flyer.Destination = hive.flowerPoints[index].position;
		GetParent().AddChild(flyer);
	}
	
	public void Pollinate(double delta) {
		if(isBloomed) {
			return;
		}
		bloomAmount += (float)delta * bloomSpeed; 
		if(bloomAmount >= 100) {
			BloomFlower();
		}
		if(bloomAmount >= nextBloom) {
			nextBloom += spawnTilesPerBloomAmount;
			SpawnTiles(chancesToSpawnTile);
		}
		float scale = bloomAmount / 200 + .5f;
		Scale = new Vector2(scale, scale);
	}

	public override void _Ready()
	{
		if(!hasSprouted) {
			sproutTime = (float)GD.RandRange(minSproutTime, maxSproutTime);
			Visible = false;
		}
		bloomParticles = (GpuParticles2D)GetNode("BloomParticles");
		nextBloom = spawnTilesPerBloomAmount;
		Sprite2D sprite = (Sprite2D)GetNode("Sprite2D");
		sprite.Texture = textures[(int)Mathf.Floor(GD.RandRange(0,  textures.Count-1))];
		float scale = bloomAmount / 200 + .5f;
		Scale = new Vector2(scale, scale);
		meadow = (Meadow)GetNode("/root/Game/MeadowTileMap");
		float sectorSize = hive.flowerSpreadDistance / hive.sectorGridSize;
		int tilesPerSector = (int)Mathf.Floor(sectorSize / 16);
		Vector2I middleTile = new Vector2I((int)Mathf.Floor(Position.X / 16), (int)Mathf.Floor(Position.Y / 16)); 
		for(int i = -tilesPerSector; i < tilesPerSector; i++) {
			for(int j = -tilesPerSector; j < tilesPerSector; j++) {
				Vector2I tile = new Vector2I(middleTile.X + i, middleTile.Y + j);
				neighboringTiles.Add(tile);
			}
		}
		SpawnTiles(chancesToSpawnTile);
	}

	public override void _Process(double delta)
	{
		if(!hasSprouted){
			sproutTimer += (float)delta;
			if(sproutTimer >= sproutTime){
				Visible = true;
				hasSprouted = true;
			}
		}
	}
}
