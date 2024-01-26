using Godot;
using System.Collections.Generic;

public class Dialogue {
	public List<string> lines = new List<string>();
	public int currentLineIndex = 0;
	public string currentLine = "";
	public string beginDialogue() {
		currentLine = lines[currentLineIndex];
		return currentLine;
	}
	public Shop speaker = null;

	public Dialogue( Shop _speaker ) {
		speaker = _speaker;
	}

	public string NextLine() {
		currentLineIndex++;
		if(currentLineIndex < lines.Count) {
			currentLine = lines[currentLineIndex];
			return lines[currentLineIndex];
		} else {
			EndDialogue();
		}
		return "";
	}

	public virtual void EndDialogue() {}
}

public class TutorialDialogue : Dialogue {
	
	public TutorialDialogue(Shop speaker) : base(speaker) {
		lines.Add("Hey, you must be the new Beekeeper!");
		lines.Add("I knew one of you would show up eventually so I set up this little shop in preparation.");
		lines.Add("There haven't been any bees in these parts since the last beekeeper left so the field hasn't been pollinated in a while.");
		lines.Add("But I'm sure once you get going this place will blossom with beauty in no time!");
		lines.Add("Say, I have a few things you might find useful.");
		lines.Add("I have this beehive here but all the bees are scared of me so they won't come out.");
		lines.Add("Go ahead and place this out in the field and see if you can get them to come out.");
		lines.Add("I also have this jar here. Don't ask me where I got it. But once the bees start producing honey you can use this to collect it.");
		lines.Add("But bee's can't start producing honey until they get some nectar so, make sure to water any flower sprouts you see.");
		lines.Add("And then come back to me because, for the right amount of honey, I'll give you just about anything. *wink*");
		lines.Add("Oh and psst. Use the 1, 2, and 3 number keys to select items in your inventory. *double wink*");
		lines.Add("Oh and space to interact. *triple wink*");
	}

	public override void EndDialogue() {
		speaker.player.beeHiveCount++;
		speaker.CloseDialogue();
		speaker.currentDialogue = null;
	}
}

public partial class Shop : Node2D
{
	public BeeKeeper player;
	public ShopUI shopUI = null;
	public Control dialogueUI = null;
	public RichTextLabel dialogueBox = null;
	public bool shopOpen = false;
	float distanceToPlayer = 0; 
	public bool canInteract = false;
	[Export]
	public float interactionDistance = 100.0f;

	public List<Dialogue> dialogues = new List<Dialogue>();
	public TutorialDialogue tutorialDialogue;
	public Dialogue currentDialogue = null;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetNode<BeeKeeper>("/root/Game/BeeKeeper");
		shopUI = GetNode<ShopUI>("/root/Game/HUD/ShopUI");
		dialogueUI = GetNode<Control>("/root/Game/HUD/DialogueUI");
		dialogueBox = GetNode<RichTextLabel>("/root/Game/HUD/DialogueUI/RichTextLabel");
		tutorialDialogue = new TutorialDialogue(this);
		currentDialogue = tutorialDialogue;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		distanceToPlayer = Position.DistanceTo(player.Position);
		if(distanceToPlayer <= interactionDistance && canInteract == false) {
			canInteract = true;
			player.canInteractWithShop = true;
		} else if(canInteract == true && distanceToPlayer > interactionDistance){
			canInteract = false;
			shopUI.CloseShop();
			player.canInteractWithShop = false;
			shopOpen = false;
		}
	}

	public override void _Input(InputEvent @event) {
		if(@event.IsActionPressed("Interact") || @event.IsActionPressed("Select")) {
			if(player.inDialogue){
				dialogueBox.Text = currentDialogue.NextLine();
			}
		}
	}

	public void CloseDialogue() {
		dialogueUI.Visible = false;
		player.inDialogue = false;
	}

	public void OpenDialogue() {
		dialogueUI.Visible = true;
		player.inDialogue = true;
	}

	public void OnInteractedWithShop() {
		if(currentDialogue != null) {
			if(player.inDialogue) {
				return;
			}
			OpenDialogue();
			dialogueBox.Text = currentDialogue.beginDialogue();
			return;
		}
		shopOpen = true;
		shopUI.OpenShop();
	}
}
