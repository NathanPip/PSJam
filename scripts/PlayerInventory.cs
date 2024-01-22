using Godot;
using System;
using System.Collections.Generic;

public enum ItemName
{
	Empty,
	Jar,
	Hive
}

public partial class PlayerInventory : Node
{
	BeeKeeper Player;
	Pickupable currentItem;

	public List<Pickupable> Inventory = new List<Pickupable>();
	[Signal]
	public delegate void ItemPickedUpWithArgumentEventHandler(Pickupable item);
	[Signal]
	public delegate void ItemDroppedWithArgumentEventHandler(Pickupable item);

	public void AddItem(Pickupable item)
	{
		for(int i=0; i<Inventory.Count; i++)
		{
			if (Inventory[i].ItemName == ItemName.Empty)
			{
				Inventory[i] = item;
				EmitSignal(SignalName.ItemPickedUpWithArgument, item);
				return;
			}
		}
	}

	public void DropItem(Pickupable item)
	{
		item.Drop(Player.Position);
		EmitSignal(SignalName.ItemDroppedWithArgument, item);
		Inventory.Remove(item);
	}

	public void ChangeCurrentItemAmount(float amount)
	{
		if(currentItem.ItemName == ItemName.Empty)
		{
			return;
		}
		if(currentItem.Amount + amount <= currentItem.maxAmount && currentItem.Amount + amount >= 0) {
			currentItem.Amount += amount;
		} else {
		 	if(currentItem.Amount + amount > currentItem.maxAmount) {
				currentItem.Amount = currentItem.maxAmount;
			}
			else if(currentItem.Amount + amount < 0) {
				currentItem.Amount = 0;
			}
		}
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Player = GetNode<BeeKeeper>("/root/Game/BeeKeeper");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
