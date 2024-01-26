using Godot;
using System;

public partial class WateringCan_UI : AnimatedSprite2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void PlayerWaterAmountChanged(float waterAmount, float waterMax){
		if(waterAmount > waterMax){
			waterAmount = waterMax;
		}
        if (waterAmount > waterMax * 0.8f)
        {
            Frame = 4;
        }
        else if (waterAmount > waterMax * 0.66f)
        {
            Frame = 3;
        }
        else if (waterAmount > waterMax * 0.33f)
        {
            Frame = 2;
        }
        else if (waterAmount > 0.0f)
        {
            Frame = 1;
        }
        else if (waterAmount <= 0.0f)
        {
            Frame = 0;
		}
	}
}
