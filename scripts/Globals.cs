using Godot;
using System;

public partial class Globals : Node 
{
	[Export]
	public static int MapSize = 2500;

	[Export]
	public static float totalHoneyCollected = 0.0f;

	[Export]
	public static int totalBeehivesPlaced = 0;
}
