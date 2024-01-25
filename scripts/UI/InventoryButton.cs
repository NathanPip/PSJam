using Godot;
using System;

public partial class InventoryButton : Button
{

	[Export]
	public EInventoryItem Item = EInventoryItem.None;
	double lerp = 0;
	bool isAnimatingUp = false;
	bool isAnimatingDown = false;

	public void AnimateUp() {
		if(Item == EInventoryItem.None) {
			return;
		}
		isAnimatingDown = false;
		isAnimatingUp = true;
	}

	public void AnimateDown() {
		if(Item == EInventoryItem.None) {
			return;
		}
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

	public void OnHoneyAmountChanged(float amount, float max) {
		if(amount >= max) {
			GetNode<Label>("Label").Text = ((int)max).ToString();
			return;
		}
		if(Item == EInventoryItem.Jar) {
			GetNode<Label>("Label").Text = ((int)amount).ToString();
		}
	}

	public void OnBeehiveCountChanged(int count) {
		if(Item == EInventoryItem.BeeHive) {
			GetNode<Label>("Label").Text = count.ToString();
			if(count == 0) {
				GetNode<AnimatedSprite2D>("AnimatedSprite2D").SelfModulate = new Color("777777FF");
			}
			else {
				GetNode<AnimatedSprite2D>("AnimatedSprite2D").SelfModulate = new Color("FFFFFFFF");
			}
		}
	}
	public void OnWaterAmountChanged(float amount, float max) {
		if(amount >= max) {
			GetNode<Label>("Label").Text = ((int)max).ToString();
			return;
		}
		if(Item == EInventoryItem.WateringCan) {
			GetNode<Label>("Label").Text = ((int)amount).ToString();
		}
	}
}
