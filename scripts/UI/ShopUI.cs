using Godot;
using System;

public partial class ShopUI : Control
{
	BeeKeeper player = null;
	Control JarItem = null;
	Control BeeHiveItem = null;

	[Export]
	public float JarPrice = 50.0f;
	[Export]
	public float BeeHivePrice = 100.0f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetNode<BeeKeeper>("/root/Game/BeeKeeper");
		JarItem = GetNode<Control>("JarItem");
		BeeHiveItem = GetNode<Control>("BeehiveItem");
		JarItem.GetNode<Label>("PurchaseButton/Label").Text = JarPrice.ToString();
		BeeHiveItem.GetNode<Label>("PurchaseButton/Label").Text = BeeHivePrice.ToString();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void UpdateShopUI() {
		if(player.honeyAmount < JarPrice) {
			JarItem.GetNode<Button>("PurchaseButton").Disabled = true;
			JarItem.GetNode<Panel>("DisabledPanel").Visible = true;
		} else {
			JarItem.GetNode<Button>("PurchaseButton").Disabled = false;
			JarItem.GetNode<Panel>("DisabledPanel").Visible = false;
		}
		if(player.honeyAmount < BeeHivePrice) {
			BeeHiveItem.GetNode<Button>("PurchaseButton").Disabled = true;
			BeeHiveItem.GetNode<Panel>("DisabledPanel").Visible = true;
		} else {
			BeeHiveItem.GetNode<Button>("PurchaseButton").Disabled = false;
			BeeHiveItem.GetNode<Panel>("DisabledPanel").Visible = false;
		}
	}

	public void OpenShop() {
		UpdateShopUI();
		Visible = true;
	}

	public void CloseShop() {
		Visible = false;
	}

	public void OnJarItemPurchased() {
		GD.Print("Jar purchased");
		if(player.honeyAmount >= JarPrice) {
			player.honeyAmount -= JarPrice;
			player.honeyMax += 100;
		}
		UpdateShopUI();
	}

	public void OnBeehiveItemPurchased() {
		GD.Print("BeeHive purchased");
	 	if(player.honeyAmount >= BeeHivePrice) {
			player.honeyAmount -= BeeHivePrice;
			player.beeHiveCount += 1;
		}
		UpdateShopUI();
	}
}
