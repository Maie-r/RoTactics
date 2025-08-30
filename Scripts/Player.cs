using Godot;
using System;
using System.Collections.Generic;

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

	public bool InvertedLook = false;
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
	public float MaxRotationSpeed = 20; // 20 is good
	[Export]
	public float Friction = 5;
	[Export]

	public int MaxHealth = 3;
	[Export]
	public int CurrentHealth;
	Timer IFrameTimer;
	public bool Immune = false;
	Timer FlashingTimer;
	
	public Sword Sword;
	public Shield Shield;

	public List<float> AngularKnockbacks = new List<float>(); // Accel
	public float TotalAngularKnockback
	{
		get {
			float res = 0;
			foreach (float angle in AngularKnockbacks)
				res += angle;
			return res;
		 }
	}
	public float BaseRotation = 0;
	public float RotationOffset = 0;

	Timer StunTimer;
	public bool Stunned = false;

	Timer LungeTimer;
	bool CanLunge = true;
	[Export]
	public float LungeCooldown = 0.4f;

	Timer ParryTimer;
	bool CanParry = true;
	Timer ParryWindowTimer;
	[Export]
	public float ParryCooldown = 1;
	public float ParryWindow = 0.2f;

	public float AngularVelocity = 0; // Positive = clockwise
	float LastRotationAngle;

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
			if (ID >= 3) //controller
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
		//GD.Print(LastRotationAngle);
		if (Input.IsActionJustPressed("test"))
			PushRotation(4f);
		else if (Input.IsActionJustPressed("test2"))
			TakeDamage();//PushRotation(-4f);
		RotationFriction((float)delta);
		SlideFriction((float)delta);
		AngularVelocity = (GlobalRotation - LastRotationAngle) / (float)delta;
		LastRotationAngle = GlobalRotation;
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
		float finalrotation = 0;
		for (int i = 0; i < AngularKnockbacks.Count; i++)
		{
			AngularKnockbacks[i] *= 0.96f; //* (float)delta;
			finalrotation += AngularKnockbacks[i];
			if (Mathf.Abs(AngularKnockbacks[i]) < 0.01)
			{
				AngularKnockbacks.RemoveAt(i);
				i--;
			}
		}
		//if (LastRotationAngle == 0) return;
		//LastRotationAngle *= 0.9f;
		//GD.Print(finalrotation);
		RotationOffset = finalrotation/Mathf.Pi;
		this.Rotation += (finalrotation ) * delta; // + (LastRotationAngle * 20)
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
				var push = (Velocity.Length() * delta) + 2;
				rb.ApplyCentralImpulse(-collision.GetNormal() * push);
				rb.ApplyCentralForce(GetAngularPush(this.Velocity, this.AngularVelocity, this.Rotation));
			}
			else if (collision.GetCollider() is CharacterBody2D cb)
			{
				var push = (Velocity.Length() * delta) + 1;
				cb.Velocity += -collision.GetNormal() * push;
			}
		}
	}

	void TryLookAt(Vector2 GoalPosition, float delta)
	{
		if (!InvertedLook)
			TryLook((GoalPosition - this.GlobalPosition).Angle(), delta);
		else
			TryLook((this.GlobalPosition - GoalPosition).Angle(), delta);
	}
	void TryLookFrom(Vector2 Axis, float delta)
	{
		TryLook(Axis.Angle(), delta);
	}

	void TryLook(float angle, float delta)
	{
		//var variation = Mathf.AngleDifference(angle2, Rotation);
		var variation = Mathf.AngleDifference(this.BaseRotation, angle);//Mathf.Abs(angle2) - Mathf.Abs(this.BaseRotation);

		//GD.Print($"{this.BaseRotation/Mathf.Pi} or {this.BaseRotation} = {angle} or {angle2}"); // BaseRotation = angle2
		//GD.Print(Mathf.Abs(variation2));
		//GD.Print($"{orientation} {orientation2}");
		if (Mathf.Abs(variation) > 0.1) // Too far
		{
			var step = MaxRotationSpeed * delta;
			var rot = Math.Clamp(variation, -step, step);
			//LastRotationAngle = rot;
			BaseRotation += rot;
		}
		else
		{
			//LastRotationAngle = angle;
			this.BaseRotation = angle;
		}
		Rotation = BaseRotation + RotationOffset;
	}
}

public partial class Player : CharacterBody2D // EVENTS
{
	public void TakeDamage()
	{
		if (!Immune)
		{
			CurrentHealth -= 1;
			if (CurrentHealth <= 0)
				Die();
			else{
				Immune = true;
				IFrameTimer.Start(2);
				StartFlashing();
			}
		}
		
	}

	public void Die()
	{
		this.QueueFree();
	}

	public void StartFlashing()
	{
		this.Visible = false;
		FlashingTimer.Start(0.15f);
	}

	public void StopFlashing()
	{
		FlashingTimer.Stop();
		this.Visible = true;
	}
	public void Lunge()
	{
		if (CanLunge)
		{
			CanLunge = false;
			LungeTimer.Start(LungeCooldown);
			Velocity = Transform.X * Accel;
			TempAudio.PlayRandomPitch("Lunge.mp3", 0.1f, this);
		}
	}

	public void Parry()
	{
		if (CanParry)
		{
			Shield.ParryActive();
			ParryWindowTimer.Start(ParryWindow);
			CanParry = false;
			ParryTimer.Start(ParryCooldown);
		}
	}

	public void ParryOver()
	{
		Shield.ParryOver();
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
		AngularKnockbacks.Add(Rotation);
		//this.Rotation += Rotation;
		//LastRotationAngle = Rotation;
	}

	void UpdateDebugMenu()
	{
		DebugDisplays[0].Text = "X: " + this.Velocity.X.ToString("N2");
		DebugDisplays[1].Text = "Y: " + this.Velocity.Y.ToString("N2");
		DebugDisplays[2].Text = "Angle: " + this.Rotation.ToString("N2");
		DebugDisplays[3].Text = Stunned ? "Stunned" : "";
	}

	static Vector2 GetAngularPush(Vector2 velocity, float angularVelocity, float FacingAngleDegrees)
	{
		Vector2 res = new Vector2(velocity.X, velocity.Y);
		var facingSin = Mathf.Sin(FacingAngleDegrees);
		var facingCos = Mathf.Cos(FacingAngleDegrees);
		res.X += facingSin * angularVelocity;
		res.Y += facingCos * angularVelocity;

		return res;
	}
}

public partial class Player : CharacterBody2D // LOADS
{
	public void SetCollisions()
	{
		SetCollisions(this, ID);
		Sword.SetCollisions(ID);
		Shield.SetCollisions(ID);
	}

	public static void SetCollisions(CollisionObject2D obj, int ID)
	{
		if (ID > 0)
		{
			uint CollisionValue = (uint)Math.Pow(2, ID + 3);
			//obj.CollisionLayer = CollisionValue;
			//obj.CollisionMask -= CollisionValue;
			//obj.CollisionMask -= (uint)Math.Pow(2, ID + 11);
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
		ParryWindowTimer.Timeout += ParryOver;
		ParryWindowTimer.OneShot = true;

		IFrameTimer = new Timer();
		IFrameTimer.Timeout += () => {Immune = false; StopFlashing();};
		IFrameTimer.OneShot = true;

		FlashingTimer = new Timer();
		FlashingTimer.Timeout += () => this.Visible = !this.Visible;
		FlashingTimer.OneShot = false;

		AddChild(StunTimer);
		AddChild(LungeTimer);
		AddChild(ParryTimer);
		AddChild(ParryWindowTimer);
		AddChild(IFrameTimer);
		AddChild(FlashingTimer);
	}
}
