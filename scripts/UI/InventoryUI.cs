using Godot;
using System;

public partial class InventoryUI : HBoxContainer
{
	InventoryButton inventoryButton1;
	InventoryButton inventoryButton2;
	InventoryButton inventoryButton3;

	InventoryButton currentButton;

	public void selectInventoryButton(InventoryButton button) {
			button.ButtonPressed = true;
			button.AnimateUp();
			if(currentButton != null) {
				currentButton.AnimateDown();
			}
			if(currentButton == button) {
				currentButton = null;
				return;
			}
			currentButton = button;
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		inventoryButton1 = GetNode<InventoryButton>("Item1");
		inventoryButton2 = GetNode<InventoryButton>("Item2");
		inventoryButton3 = GetNode<InventoryButton>("Item3");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnInventoryChanged(int item) {
		if(item == 0) {
			selectInventoryButton(currentButton);
		}
		if(item == 1) {
			selectInventoryButton(inventoryButton1);
		}
		if(item == 2) {
			selectInventoryButton(inventoryButton2);
		}
		if(item == 3) {
			selectInventoryButton(inventoryButton3);
		}
	}
}
