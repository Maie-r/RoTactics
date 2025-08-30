using Godot;
using System;
using System.Collections.Generic;
/*
public partial class SwordV3 : Area2D //Actually SwordV2
{
	[Export] public float PoleRadius = 64f;   // approximate physical radius
	[Export] public float SpinCoupling = 0.7f;
	[Export] public float SeparationStrength = 2000f;
	public Player playerRef;
	Vector2 offset;

	Vector2 LastPos;
	float LastRotationAngle;

	Vector2 Velocity;
	float AngularVelocity = 0; // Positive = clockwise

	private readonly HashSet<RigidBody2D> _CollidedBodies = new();

	private void _on_body_entered(Node2D Body)
	{
		GD.Print("Sword: BODY ENTERED: " + Body.Name);
		if (Body is Sword sw) // remove later
			SwordOnSword(sw);
		else if (Body is Shield sd)
			SwordOnShield(sd);
		else if (Body is RigidBody2D rb)
			SwordOnObject(rb);
	}

	private void _on_area_entered(Area2D area)
	{
		GD.Print("Sword: AREA ENTERED: " + area.Name);
		if (area is ParryArea)
		{
			TempAudioGlobal.Play("hit_critPLACEHOLDER.mp3", this);
		}
	}

	private void _on_parry_trigger_body_entered(Node2D Body)
	{
		//GD.Print("Sword Parry Trigger: BODY ENTERED: " + Body.Name);
		if (Body is Sword sw)
			SwordOnSword(sw);
		else if (Body is Shield sd)
			SwordOnShield(sd);
	}

	private void _on_parry_trigger_area_entered(Area2D area)
	{
		//GD.Print("Sword Parry Triger: AREA ENTERED: " + area.Name);
		if (area is ParryArea)
		{
			TempAudioGlobal.Play("hit_critPLACEHOLDER.mp3", this);
		}
	}

	public override void _Ready()
	{
		BodyEntered += b => { if (b is RigidBody2D rb) _CollidedBodies.Add(rb); };
		BodyExited  += b => { if (b is RigidBody2D rb) _CollidedBodies.Remove(rb); };
		offset = Position;
		LastPos = GlobalPosition;
		LastRotationAngle = GlobalRotation;
	}

	public override void _Process(double delta)
	{
		foreach (var rb in _CollidedBodies)
		{
			Vector2 r = rb.GlobalPosition - GlobalPosition;
			if (r.LengthSquared() < 1e-6f) continue;

			// 1. Tangential fling: vt = ω × r
			Vector2 vt = AngularVelocity * new Vector2(-r.Y, r.X);
			Vector2 dv = (vt - rb.LinearVelocity) * SpinCoupling;
			rb.ApplyImpulse(rb.Mass * dv);

			// 2. Separation push: outward if inside pole radius
			float dist = r.Length();
			if (dist < PoleRadius)
			{
				float depth = PoleRadius - dist;
				Vector2 outward = r.Normalized();

				Vector2 sepImpulse = outward * depth * SeparationStrength * (float)delta;
				rb.ApplyImpulse(sepImpulse);
			}
		}
		Velocity = (GlobalPosition - LastPos) / (float)delta;
		AngularVelocity = (GlobalRotation - LastRotationAngle) / (float)delta;
		LastPos = GlobalPosition;
		LastRotationAngle = GlobalRotation;
		//GD.Print("Velocity: " + Velocity);
		//GD.Print("Angular Velocity: " + AngularVelocity);
		/*
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			var collision2 = GetSlideCollision(i);
			if (collision2.GetCollider() is RigidBody2D rb)
			{
				var push = (Velocity.Length() * (float)delta) + 1;
				rb.ApplyCentralImpulse(-collision2.GetNormal() * push);
			}
			else
				GD.Print("UNHANDLED");
		}//*//*
		var target = playerRef.GlobalPosition;
		var motion = target - this.GlobalPosition;
		var collision = MoveAndCollide(motion);
		if (collision != null)
			HandleCollision(collision);*//*

	}

	void HandleCollision(KinematicCollision2D collision)
	{
		//GD.Print(collision);
		var normal = collision.GetNormal();
		GD.Print(normal);
		var depth = collision.GetRemainder().Length();
		var pushback = normal * depth * 50;  //tweak multiplier
		playerRef.Push(pushback);
	}

	void SwordOnSword(Sword sw)
	{
		Vector2 pushdir = Vector2.FromAngle(this.GlobalRotation + Rotation);
		GD.Print(pushdir);
		sw.playerRef.Push(pushdir * 300);
		sw.playerRef.Stun(0.1f);
		sw.playerRef.PushRotation(this.playerRef.LastRotationAngle);
		this.playerRef.Stun(0.1f);
		//GD.Print("SWORD ON SWORD CONTACT");
	}

	void SwordOnShield(Shield sd)
	{
		Vector2 pushdir = Vector2.FromAngle(this.GlobalRotation + Rotation);
		sd.playerRef.Push(pushdir * 100);
		//GD.Print("SWORD ON SHIELD CONTACT");
	}

	void SwordOnObject(RigidBody2D rb)
	{
		//rb.AngularVelocity += this.AngularVelocity;
		//rb.LinearVelocity += GetAngularPush(Velocity, AngularVelocity, GlobalRotation);
		
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
*/