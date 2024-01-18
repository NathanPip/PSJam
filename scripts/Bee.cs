using Godot;
using System;
using System.Collections.Generic;

public class State
{
	protected Node2D owner;
	public State(Node2D owner)
	{
		this.owner = owner;
	}

	public virtual void Enter(){}

	public virtual void Exit(){}

	public virtual void Update(double delta) {}

	public virtual void PhysicsUpdate(double delta) {}
}

public class BeeIdleState : State 
{
	public float moveChance = .3f;
	Bee bee;

	public BeeIdleState(Bee owner) : base(owner) {
		bee = (Bee)owner;
	}

	public void restartState() {
		Bee bee = (Bee)this.owner;
		if(bee.hive == null) {
			GD.Print("Bee has no hive!!!");
			return;
		}
		float searchRand = GD.Randf();
		if (searchRand < .2){
			bee.ChangeState(bee.states[1]);
			return;
		}
		float moveRand = GD.Randf();
		if (moveRand < moveChance){
			float randomRotation = GD.Randf() * -Mathf.Pi * 2 + Mathf.Pi;
            float moveDistance = GD.Randf() * bee.hive.beeMoveRange;
            Vector2 moveLocation = bee.hive.Position + new Vector2(1, 0).Rotated(randomRotation) * moveDistance;
			bee.MoveTo(moveLocation);
			return;
		}
		float rotationAmt = owner.Rotation + GD.Randf() * -Mathf.Pi + Mathf.Pi / 2;
		bee.RotateTo(rotationAmt);
	}

	public override void Enter() {
		restartState();
	}

	public override void Exit() {}
	
	public override void Update(double delta) {
		bee = (Bee)this.owner; 
		if (!bee.isMoving && !bee.isRotating){
			restartState();
		}
	}
}

public class BeeCollectingState : State {

	Bee bee;
	Flower flower;
	bool movingToFlower = false;
	public BeeCollectingState(Bee owner) : base(owner) {
		bee = (Bee)owner;
	}

	public override void Enter() {
		Flower flower = bee.hive.flowers[GD.RandRange(0, bee.hive.flowers.Count - 1)]; 
		bee.MoveTo(flower.Position);
		movingToFlower = true;
	}

	public override void Update(double delta) {
		if(movingToFlower && !bee.isMoving) {
			bee.ChangeState(bee.states[0]);
		}
	}
}

public class BeePollinatingState : State {
	
	public BeePollinatingState(Bee owner) : base(owner) {}
}

public partial class Bee : Node2D 
{

	[Export]
	public float RotationSpeed = 5;
	[Export]
	public float MovementSpeed = 40;
	[Export]
	public BeeHive hive = null;

	Vector2 moveToLocation;
	float lookAtRotation;
	public bool isMoving = false;
	public bool isRotating = false;

	public List<State> states = new List<State>();
	State currentState;

	public void ChangeState(State newState){
		if(currentState != null){
			currentState.Exit();
		}
		currentState = newState;
		currentState.Enter();
	}

	public void MoveTo(Vector2 location){
		moveToLocation = location;
		isMoving = true;
	}

	public void RotateTo(float rotation){
		lookAtRotation = rotation;
		isRotating = true;
	}

	public void _Move(double delta) {
			Position = Position.MoveToward(moveToLocation, (float)delta * MovementSpeed);
			Rotation = Mathf.Lerp(Rotation, Position.AngleToPoint(moveToLocation), (float)delta * RotationSpeed);
			if(Position.DistanceTo(moveToLocation) < 5){
				isMoving = false;
				return;
			}
			Position += new Vector2(1, 0).Rotated(Rotation) * (float)delta * MovementSpeed / 3;
	}

	public void _Rotate(double delta) {
			Rotation = Mathf.Lerp(Rotation, lookAtRotation, (float)delta * 3);
			if(Mathf.Abs(Rotation - lookAtRotation) < .1){
				isRotating = false;
			}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BeeIdleState idleState = new BeeIdleState(this);
		BeeCollectingState collectingState = new BeeCollectingState(this);
		BeePollinatingState pollinatingState = new BeePollinatingState(this);
		states.Add(idleState);
		states.Add(collectingState);
		states.Add(pollinatingState);
		ChangeState(idleState);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(isMoving){
			_Move(delta);
		}
		if(isRotating){
			_Rotate(delta);
		}
		if(currentState != null){
			currentState.Update(delta);
		}
	}

    public override void _PhysicsProcess(double delta)
    {
		if(currentState != null){
			currentState.PhysicsUpdate(delta);
		}
    }
}
