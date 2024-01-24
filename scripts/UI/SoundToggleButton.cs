using Godot;

public partial class SoundToggleButton : Button
{

	public void OnMusicToggleButtonPressed() {
	 	if (AudioServer.IsBusMute(AudioServer.GetBusIndex("Master")))
		{
			AudioServer.SetBusMute(AudioServer.GetBusIndex("Master"), false);
		}
		else
		{
			AudioServer.SetBusMute(AudioServer.GetBusIndex("Master"), true);
		}
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pressed += OnMusicToggleButtonPressed;
	}

}
