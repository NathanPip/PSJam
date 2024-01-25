using Godot;

public partial class MusicToggleButton : TextureButton
{
	public void OnMusicToggleButtonPressed() {
	 	if (AudioServer.IsBusMute(AudioServer.GetBusIndex("Music")))
		{
			AudioServer.SetBusMute(AudioServer.GetBusIndex("Music"), false);
		}
		else
		{
			AudioServer.SetBusMute(AudioServer.GetBusIndex("Music"), true);
		}
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pressed += OnMusicToggleButtonPressed;
	}
}
