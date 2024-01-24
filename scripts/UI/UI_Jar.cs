using Godot;
using System;

public partial class UI_Jar : AnimatedSprite2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void PlayerHoneyAmountChanged(float honeyAmount, float honeyMax){
		if(honeyAmount > honeyMax){
			honeyAmount = honeyMax;
		}
        if (honeyAmount > honeyMax * 0.9f)
        {
            Frame = 4;
        }
        else if (honeyAmount > honeyMax * 0.75f)
        {
            Frame = 3;
        }
        else if (honeyAmount > honeyMax * 0.33f)
        {
            Frame = 2;
        }
        else if (honeyAmount > 0.0f)
        {
            Frame = 1;
        }
        else if (honeyAmount <= 0.0f)
        {
            Frame = 0;
		}
	}
}
