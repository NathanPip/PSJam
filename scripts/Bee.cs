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
		if(bee.nectarAmt <= bee.nectarCapacity / 2 && !bee.hive.isFull) {
            float searchRand = GD.Randf();
            if (searchRand < .2)
            {
                bee.ChangeState(bee.states[1]);
                return;
            }
		} else if (bee.nectarAmt >= bee.nectarCapacity / 2 && !bee.hive.isFull) {
		 	bee.ChangeState(bee.states[2]);
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
	bool collectingNectar = false;
	float timeToCollect = 0;
	float timer = 0;
	public BeeCollectingState(Bee owner) : base(owner) {
		bee = (Bee)owner;
	}

	public void Collect() {
		timeToCollect = GD.Randf() * 3 + 1;
	}

	public override void Enter() {
		int flowerCount = bee.hive.flowers.Count;
		if(flowerCount == 0){
			bee.ChangeState(bee.states[0]);
			return;
		}
		flower = bee.hive.flowers[GD.RandRange(0, bee.hive.flowers.Count - 1)]; 
		if(flower.isBloomed || !flower.hasSprouted || !flower.watered && !bee.hive.allFlowersBloomed) {
			bee.ChangeState(bee.states[0]);
			return;
		}
		bee.MoveTo(flower.Position);
		movingToFlower = true;
	}

	public override void Exit() {
		timer = 0;
		timeToCollect = 0;
		collectingNectar = false;
		movingToFlower = false;
		flower = null;
	}

	public override void Update(double delta) {
		if(movingToFlower && !bee.isMoving) {
			movingToFlower = false;
			Collect();
			collectingNectar = true;
			return;
		}
		if(collectingNectar) {
			float previousTime = timer;
			timer += (float)delta;
			if(timer >= timeToCollect) {
				collectingNectar = false;
				flower.Pollinate(timeToCollect - previousTime);
				bee.CollectNectar(timeToCollect - previousTime);
				bee.ChangeState(bee.states[2]);
				return;
			}
			flower.Pollinate(delta);
			bee.CollectNectar((float)delta);
		}
	}
}

public class BeePollinatingState : State {
	
	Bee bee;
	public bool movingToHive = false;
	public bool makingHoney = false;
	float previousPollination = 2f;
	float pollinationTimer = 0;
	public BeePollinatingState(Bee owner) : base(owner) {
		bee = (Bee)owner;
	}

	public override void Enter() {
	 	bee.MoveTo(bee.hive.Position);
		movingToHive = true;
		bee.PollenParticles.Emitting = true;
	}
	
	public override void Exit() {
	 	movingToHive = false;
		makingHoney = false;
		bee.BeeSprite.Visible = true;
	}

	public override void Update(double delta) {
		if(movingToHive && bee.isMoving) {
			pollinationTimer += (float)delta;
			if(pollinationTimer >= previousPollination) {
				float rand = GD.Randf();
				if(rand < .1) {
					// Vector2 flowerPosition = bee.Position + new Vector2(0, 1).Rotated(bee.Rotation + Mathf.Pi / 2) * 50 + 
					// 	new Vector2(GD.Randf() * 100 - 50, GD.Randf() * 100 - 50);
					// bee.hive.AddFlower(flowerPosition);
					float sectorSize = bee.hive.flowerSpreadDistance / bee.hive.sectorGridSize;
					int XGridPosition = (int)Math.Min((int)Mathf.Floor(
								((bee.Position.X - bee.hive.Position.X) + bee.hive.flowerSpreadDistance/2) / sectorSize), bee.hive.sectorGridSize - 1);
					int YGridPosition = (int)Math.Min((int)Mathf.Floor(
								((bee.Position.Y - bee.hive.Position.Y) + bee.hive.flowerSpreadDistance/2) / sectorSize), bee.hive.sectorGridSize - 1);
					int index = YGridPosition * (int)bee.hive.sectorGridSize + XGridPosition;
					if(index < bee.hive.flowerPoints.Count){
                        FlowerPoint flowerPoint = bee.hive.flowerPoints[index];
                        if (!flowerPoint.spawned)
                        {
                            bee.hive.SpawnFlowerAtPointIndex(index);
                        }
					}
				}
				previousPollination += 2f;
			}
			return;
		}
		if(movingToHive && !bee.isMoving) {
			movingToHive = false;
			makingHoney = true;
			bee.PollenParticles.Emitting = false;
			bee.BeeSprite.Visible = false;
			return;
		}
		if(makingHoney) {
			if(bee.hive.isFull) {
			 	bee.ChangeState(bee.states[0]);
				return;
			}
			float dispenseAmt = (float)delta;
			float previousNectar = bee.nectarAmt;
			bee.nectarAmt -= dispenseAmt;
			if(bee.nectarAmt <= 0) {
				bee.hive.StoreHoney(previousNectar);
				bee.nectarAmt = 0;
				bee.ChangeState(bee.states[0]);
				return;
			}
			bee.hive.StoreHoney(dispenseAmt);
		}
	}
}

public partial class Bee : Node2D 
{

	[Export]
	public float RotationSpeed = 5;
	[Export]
	public float MovementSpeed = 40;
	[Export]
	public BeeHive hive = null;
	[Export]
	public float nectarAmt = 0;
	[Export]
	public float nectarCapacity = 10;
	[Export]
	public float collectionRate = 5;

	Vector2 moveToLocation;
	public GpuParticles2D PollenParticles;
	public Sprite2D BeeSprite;
	float lookAtRotation;
	public bool isMoving = false;
	public bool isRotating = false;

	public List<State> states = new List<State>();
	State currentState;

	public void CollectNectar(float delta) {
		nectarAmt += delta * collectionRate;
	}

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
		PollenParticles = GetNode<GpuParticles2D>("PollenParticles");
		BeeSprite = GetNode<Sprite2D>("BeeSprite");
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
