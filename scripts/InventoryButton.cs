using Godot;
using System;

public partial class InventoryButton : Button
{

	double lerp = 0;
	bool isAnimatingUp = false;
	bool isAnimatingDown = false;

	public void AnimateUp() {
		isAnimatingDown = false;
		isAnimatingUp = true;
	}

	public void AnimateDown() {
		isAnimatingUp = false;
		isAnimatingDown = true;
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(isAnimatingUp && lerp < 1) {
			lerp += delta * 5;
			lerp = Mathf.Clamp(lerp, 0, 1);
			Position = new Vector2(Position.X, Mathf.Lerp(0, -10, (float)lerp));
			if(lerp >= 1) {
				isAnimatingUp = false;
				lerp = 1;
			}
		}
		else if(isAnimatingDown && lerp > 0) {
			lerp -= delta * 5;
			lerp = Mathf.Clamp(lerp, 0, 1);
			Position = new Vector2(Position.X, Mathf.Lerp(0, -10, (float)lerp));
			if(lerp <= 0) {
				isAnimatingDown = false;
				lerp = 0;
			}
		}
	}
}
