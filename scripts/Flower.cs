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
	public float sproutTimer = 0;
	public float sproutTime = 0;
	public bool isBloomed = false;
	public float bloomAmount = 1;
	public BeeHive hive = null;
	public Meadow meadow = null;

	Texture2D MedusaFlower = (Texture2D)ResourceLoader.Load("res://assets/sprites/Medusaflower.png");
	Texture2D BlackEyeSusanFlower = (Texture2D)ResourceLoader.Load("res://assets/sprites/blackeyesusan.png");
	Texture2D BlueFlower = (Texture2D)ResourceLoader.Load("res://assets/sprites/blueflower.png");
	Texture2D MouthFlower = (Texture2D)ResourceLoader.Load("res://assets/sprites/mouthflower.png");
	Texture2D Shroom = (Texture2D)ResourceLoader.Load("res://assets/sprites/shroom.png");

	public List<Texture2D> textures = new List<Texture2D>() {
	 	(Texture2D)ResourceLoader.Load("res://assets/sprites/Medusaflower.png"),
		(Texture2D)ResourceLoader.Load("res://assets/sprites/blackeyesusan.png"),
		(Texture2D)ResourceLoader.Load("res://assets/sprites/blueflower.png"),
		(Texture2D)ResourceLoader.Load("res://assets/sprites/mouthflower.png"),
	};
	
	public void Pollinate(double delta) {
		if(isBloomed) {
			return;
		}
		bloomAmount += (float)delta * bloomSpeed; 
		if(bloomAmount >= 100) {
			bloomAmount = 100;
			isBloomed = true;
			for(int i=0; i<hive.flowerPoints.Count; i++) {
				if(hive.flowerPoints[i].spawned == false) {
					hive.SpawnFlowerAtPointIndex(i);
					hive.flowersBloomed++;
					break;
				}
			}
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
		Sprite2D sprite = (Sprite2D)GetNode("Sprite2D");
		sprite.Texture = textures[(int)Mathf.Floor(GD.RandRange(0,  textures.Count-1))];
		float scale = bloomAmount / 200 + .5f;
		Scale = new Vector2(scale, scale);
		meadow = (Meadow)GetNode("/root/Game/MeadowTileMap");
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
