using Godot;
using System;

public partial class Player : CharacterBody2D
{
	//DEBUG EXPORTS
	[Export]
	public bool NoInteractables = false;
	[Export]
	public bool InstantTurning = false;
	[Export]
	public bool DisableMovement = false;
	[Export]
	public bool DebugDisplay = false;
	CollisionObject2D box;
	Label[] DebugDisplays = new Label[4];
	////////////////


	[Export]
	public int TeamID;
	public Team team;
	[Export(PropertyHint.Range, "1,4,1")]
	public int ID = 1;

	[Export]
	public float MaxSpeed = 1000;
	[Export]
	public float Accel = 2000;
	[Export]
	public float MaxRotationSpeed = 20;
	[Export]
	public float Friction = 5;
	[Export]
	public int MaxHealth = 3;
	[Export]
	public int CurrentHealth;

	public Sword Sword;
	public Shield Shield;

	public float CurrentRotation = 0;
	public float LastRotationAngle = 0;

	Timer StunTimer;
	public bool Stunned = false;

	Timer LungeTimer;
	bool CanLunge = true;

	Timer ParryTimer;
	bool CanParry = true;
	Timer ParryWindowTimer;

	public override void _Ready()
	{
		Sword = GetNode<Sword>("Sword");
		Shield = GetNode<Shield>("Shield");
		Sword.playerRef = this;
		Shield.playerRef = this;
		if (NoInteractables)
		{
			Sword.QueueFree();
			Shield.QueueFree();
		}
		SetCollisions();
		if (DebugDisplay)
		{
			DebugDisplays[0] = GetNode<Label>("DebugDisplay/VelocityDisplay/X vel");
			DebugDisplays[1] = GetNode<Label>("DebugDisplay/VelocityDisplay/Y vel");
			DebugDisplays[2] = GetNode<Label>("DebugDisplay/AngleDisplay/Angle");
			DebugDisplays[3] = GetNode<Label>("DebugDisplay/Status/Stunned");
		}
		SetTimers();
		//box = ResourceLoader.Load<PackedScene>("res://Scenes/MovableBox.tscn").Instantiate<CollisionObject2D>();
		//box.GlobalPosition = GetGlobalMousePosition();
		//AddChild(box);
		CurrentHealth = MaxHealth;
	}

	/*public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey IK)
		{
			if (IK.Keycode == Key.Z)
			{
				Lunge();
			}
			else if (IK.Keycode == Key.X)
			{
				Parry();
			}
		}
		else if (@event is InputEventMouseButton IM)
		{
			if (IM.ButtonIndex == MouseButton.Left)
			{
				Lunge();
			}
			else if (IM.ButtonIndex == MouseButton.Right)
			{
				Parry();
			}
		}
	}*/

	public override void _Process(double delta)
	{
		if (!Stunned && !DisableMovement)
		{
			if (ID >= 3)
			{
				var axis = Input.GetVector($"lleft_{ID}", $"lright_{ID}", $"lup_{ID}", $"ldown_{ID}");
				if (axis.Length() > 0.2f)
				{
					if (InstantTurning)
						TryLookFrom(axis, 2);
					else
						TryLookFrom(axis, (float)delta);
				}
			}
			else
			{
				if (InstantTurning)
					LookAt(GetGlobalMousePosition());
				else
					TryLookAt(GetGlobalMousePosition(), (float)delta);
			}


			//box.GlobalPosition = GetGlobalMousePosition();
			SingleInputs();
			MoveInputs((float)delta);
		}
		else
		{
			if (Input.IsActionJustPressed("test"))
				PushRotation(0.8f);
			else
				RotationFriction((float)delta);
		}
			
		SlideFriction((float)delta);
		MoveAndSlide();
		CheckCollisions((float)delta);
		if (DebugDisplay)
			UpdateDebugMenu();
	}

	void MoveInputs(float delta)
	{
		Vector2 vel = Velocity;
		if (!DisableMovement)
		{
			Vector2 direction = Input.GetVector($"left_{ID}", $"right_{ID}", $"up_{ID}", $"down_{ID}");
			if (direction != Vector2.Zero)
			{
				vel += direction * Accel * delta;
			}
		}
		Velocity = vel;
	}

	void SlideFriction(float delta)
	{
		Vector2 vel = Velocity;
		vel = vel.LimitLength(MaxSpeed);
		if (vel.Length() > delta * Friction)
			vel -= vel * Friction * delta;
		else
			vel = Vector2.Zero;
		Velocity = vel;
	}

	void RotationFriction(float delta)
	{
		if (LastRotationAngle == 0) return;
		LastRotationAngle *= 0.9f;
		this.Rotation += LastRotationAngle * 2 * delta;
	}

	void SingleInputs()
	{
		if (Input.IsActionJustPressed($"parry_{ID}"))
		{
			Parry();
		}
		if (Input.IsActionJustPressed($"lunge_{ID}"))
		{
			Lunge();
		}
	}

	void CheckCollisions(float delta)
	{
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			var collision = GetSlideCollision(i);
			if (collision.GetCollider() is RigidBody2D rb)
			{
				var push = (Velocity.Length() * delta) + 1;
				rb.ApplyCentralImpulse(-collision.GetNormal() * push);
			}
		}
	}

	void TryLookAt(Vector2 GoalPosition, float delta)
	{
		TryLook((GoalPosition - GlobalPosition).Angle(), delta);
	}
	void TryLookFrom(Vector2 Axis, float delta)
	{
		TryLook(Axis.Angle(), delta);
	}

	void TryLook(float angle, float delta)
	{
		//var variation = Mathf.AngleDifference(angle2, Rotation);
		var variation = Mathf.AngleDifference(Rotation, angle);//Mathf.Abs(angle2) - Mathf.Abs(this.Rotation);

		//GD.Print($"{this.Rotation/Mathf.Pi} or {this.Rotation} = {angle} or {angle2}"); // Rotation = angle2
		//GD.Print(Mathf.Abs(variation2));
		//GD.Print($"{orientation} {orientation2}");
		if (Mathf.Abs(variation) > 0.5)
		{
			var step = MaxRotationSpeed * delta;
			var rot = Math.Clamp(variation, -step, step);
			LastRotationAngle = rot;
			Rotation += rot;
		}
		else
		{
			LastRotationAngle = angle;
			this.Rotation = angle;
		}
			
		CurrentRotation = this.Rotation;
	}
}

public partial class Player : CharacterBody2D // EVENTS
{
	public void Lunge()
	{
		if (CanLunge)
		{
			CanLunge = false;
			LungeTimer.Start(0.2f);
			Velocity = Transform.X * Accel;
		}
	}

	public void Parry()
	{
		if (CanParry)
		{
			Shield.ParryActive();
			ParryWindowTimer.Start(0.2f);
			CanParry = false;
			ParryTimer.Start(1);
		}
	}

	public void Stun(float Duration)
	{
		Stunned = true;
		StunTimer.Start(Duration);
	}

	public void Push(Vector2 knockback)
	{
		Velocity = knockback;
	}

	public void PushRotation(float Rotation)
	{
		CurrentRotation = Rotation;
		Rotation += Rotation;
		LastRotationAngle = Rotation;
	}

	void UpdateDebugMenu()
	{
		DebugDisplays[0].Text = "X: " + this.Velocity.X.ToString("N2");
		DebugDisplays[1].Text = "Y: " + this.Velocity.Y.ToString("N2");
		DebugDisplays[2].Text = "Angle: " + this.Rotation.ToString("N2");
		DebugDisplays[3].Text = Stunned ? "Stunned" : "";
	}
}

public partial class Player : CharacterBody2D // LOADS
{
	public void SetCollisions()
	{
		SetCollisions(this, ID - 1);
		SetCollisions(Sword, ID - 1);
		SetCollisions(Shield, ID - 1);
		SetCollisions(Sword.GetNode<Area2D>("ParryTrigger"), ID - 1);
		SetCollisions(Shield.GetNode<Area2D>("ParryTrigger"), ID - 1);
	}

	public static void SetCollisions(CollisionObject2D obj, int ID)
	{
		if (ID > 0)
		{
			uint CollisionValue = (uint)Math.Pow(2, ID);
			obj.CollisionLayer = CollisionValue;
			var Mask = obj.CollisionMask;
			obj.CollisionMask = Mask - CollisionValue;
		}
	}

	void SetTimers()
	{
		StunTimer = new Timer();
		StunTimer.Timeout += () => Stunned = false;
		StunTimer.OneShot = true;

		LungeTimer = new Timer();
		LungeTimer.Timeout += () => CanLunge = true;
		LungeTimer.OneShot = true;

		ParryTimer = new Timer();
		ParryTimer.Timeout += () => CanParry = true;
		ParryTimer.OneShot = true;

		ParryWindowTimer = new Timer();
		ParryWindowTimer.Timeout += Shield.ParryOver;
		ParryWindowTimer.OneShot = true;

		AddChild(StunTimer);
		AddChild(LungeTimer);
		AddChild(ParryTimer);
		AddChild(ParryWindowTimer);
	}
}
