using Godot;
using System;
using System.Collections;
/*
public partial class SwordBKP : CharacterBody2D //Actually SwordV1
{
	[Export]
	public bool Active = true;
	public Player playerRef;
	Vector2 offset;
	private void _on_parry_trigger_body_entered(Node2D Body)
	{
		GD.Print("Sword: BODY ENTERED: " + Body.Name);
		if (Body is Sword sw)
			SwordOnSword(sw);
		else if (Body is Shield sd)
			SwordOnShield(sd);
	}

	private void _on_parry_trigger_area_entered(Area2D area)
	{
		GD.Print("Sword: AREA ENTERED: " + area.Name);
		if (area is ParryArea)
		{
			TempAudioGlobal.Play("hit_critPLACEHOLDER.mp3", this);
		}
	}

	public override void _Ready()
	{
		if (!Active)
			QueueFree();
		offset = Position;
	}

	public override void _Process(double delta)
	{
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
			HandleCollision(collision);

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
}*/
