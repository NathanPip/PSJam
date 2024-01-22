using Godot;
using System;

public partial class Pickupable : PlayerInteractable 
{
	[Export]
	public ItemName ItemName;
	[Export]
	public float Amount;
	public float maxAmount;
	public Sprite2D Sprite;
	public AnimatedSprite2D AnimatedSprite;
	public bool active {
		get {
			if(Sprite != null)
			{
				return Sprite.Visible;
			}
			else if(AnimatedSprite != null)
			{
				return AnimatedSprite.Visible;
			}
			return false;
		}
		set {
			Sprite.Visible = value;
			AnimatedSprite.Visible = value;
		}
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		for(int i=0; i<GetParent().GetChildCount(); i++)
		{
			if(GetParent().GetChild(i) is Sprite2D)
			{
				Sprite = (Sprite2D)GetParent().GetChild(i);
			}
			if(GetParent().GetChild(i) is AnimatedSprite2D)
			{
				AnimatedSprite = (AnimatedSprite2D)GetParent().GetChild(i);
			}
		}
	}

	public void Drop(Vector2 position)
	{
		GetParent<Node2D>().Position = position;
		active = true;
	}

	public override void Interact()
	{
		if(active)
		{
			PlayerInventory inventory = (PlayerInventory)GetNode("/root/Game/BeeKeeper/PlayerInventory");
			inventory.AddItem(this);
			active = false;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
}
