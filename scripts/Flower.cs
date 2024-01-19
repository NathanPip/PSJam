using Godot;
using System;

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
	
	public void Pollinate(double delta) {
		bloomAmount += (float)delta * bloomSpeed; 
		float scale = bloomAmount / 200 + .5f;
		Scale = new Vector2(scale, scale);
		if(bloomAmount >= 100) {
			isBloomed = true;
		}
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if(!hasSprouted) {
			sproutTime = (float)GD.RandRange(minSproutTime, maxSproutTime);
			Visible = false;
		}
		float scale = bloomAmount / 200 + .5f;
		Scale = new Vector2(scale, scale);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
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
