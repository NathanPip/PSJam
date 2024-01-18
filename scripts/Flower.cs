using Godot;
using System;

public partial class Flower : Node2D
{
	[Export]
	public float bloomSpeed = 5;
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
		float scale = bloomAmount / 200 + .5f;
		Scale = new Vector2(scale, scale);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
