using Godot;
using System;

public partial class MovableBox : RigidBody2D
{
	[Export]
	public float SlideFriction = 1;
	[Export]
	public float MaxSpeed = 500;
	public float LastRotationAngle = 0;
	void ApplyRotationFriction(float delta)
	{
		if (LastRotationAngle == 0) return;
		LastRotationAngle *= 0.8f;
		this.AngularVelocity += LastRotationAngle * 2 * delta;
	}

	void ApplySlideFriction(float delta)
	{
		Vector2 vel = new Vector2(LinearVelocity.X, LinearVelocity.Y);
		vel = vel.LimitLength(MaxSpeed);
		var Friction = PhysicsMaterialOverride.Friction;
		if (vel.Length() > delta * Friction)
			vel = vel/SlideFriction * Friction * delta;
		else
			vel = Vector2.Zero;
		this.ApplyImpulse(vel*-1);
	}

	public override void _Process(double delta)
	{
		ApplySlideFriction((float)delta);
		ApplyRotationFriction((float)delta);
		if (LinearVelocity.Length() > MaxSpeed) // shouldn't be a problem right? this would get run once right?
		{
			LinearVelocity = LinearVelocity.LimitLength(MaxSpeed);
		}
			
	}
}
