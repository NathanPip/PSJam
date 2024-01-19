using Godot;
using System;

public partial class Meadow : TileMap
{
	[Export]
	float grassSpawnChance = .92f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		int MapSize = GetNode<Game>("/root/Game").MapSize;
		int limit = MapSize / 2;
		for(int i=-limit; i<limit; i++) {
			for(int j=-limit; j<limit; j++) {
				float chance = GD.Randf();
				if(chance > grassSpawnChance) {
					continue;
				}
				SetCell(1, new Vector2I(i, j) , 1, new Vector2I(GD.RandRange(0,3), 0));
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
