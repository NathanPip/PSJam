using Godot;
using System;

public partial class BloomFlyer : Node2D
{

	public Vector2 Destination;
	[Export]
	public float MaxSpeed = 100;
	public float Speed = 0;
	[Export]
	public float Acceleration = 40;
	public bool ReachedDestination = false;
	public bool Bloomed = false;
	GpuParticles2D flyingParticles = null;
	GpuParticles2D bloomParticles = null;
	public BeeHive hive = null;
	public FlowerPoint flowerPoint;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		flyingParticles = (GpuParticles2D)GetNode("FlyingParticles");
		bloomParticles = (GpuParticles2D)GetNode("BloomParticles");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(hive == null) {
			QueueFree();
		}
		if(ReachedDestination){
			flyingParticles.Emitting = false;
			if(!bloomParticles.Emitting){
				if(Bloomed) {
					QueueFree();
				}
				bloomParticles.Emitting = true;
				Bloomed = true;
				if(flowerPoint.spawned) {
					return;
				}
				hive.SpawnFlowerAtPoint(flowerPoint);
			}
		}
		Speed += Acceleration * (float)delta;
		Speed = Mathf.Clamp(Speed, 0, MaxSpeed);
		Position = Position.MoveToward(Destination, Speed * (float)delta);
		if(Position.IsEqualApprox(Destination)) {
			ReachedDestination = true;
		}
	}
}
