using Godot;
using System;

public partial class Bee : RigidBody2D
{
	[Export]
	public int UpSpeed = 100;
	[Export]
	public int HorizontalSpeed = 100;

	public bool FlyingUp = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionPressed("Fly"))
		{
			FlyingUp = true;
		}
	}

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (FlyingUp)
        {
			GD.Print("Flying up");
            ApplyCentralForce(new Vector2(0, -UpSpeed));
        }
    }
}
