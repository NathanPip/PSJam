using Godot;
using System;

public partial class PlayerInteractable : Node 
{

	public Node2D Parent;
	public bool IsInteractable = false;

	public virtual void Interact()
	{}

	public virtual void InteractWith(Node node)
	{}

	public virtual void Uninteract()
	{}

	public override void _Ready()
	{
		Parent = GetParent<Node2D>();
	}

}
