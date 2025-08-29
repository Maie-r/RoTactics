using Godot;
using System;

public partial class Shield : CharacterBody2D
{
	public Player playerRef;
	bool ParryOn = false;
	private void _on_parry_trigger_body_entered(Node2D Body)
	{
		GD.Print("Shield: BODY ENTERED: "+ Body.Name);
		if (Body is Sword sw)
			ShieldOnSword(sw);
		else if (Body is Shield sd)
			ShieldOnShield(sd);
	}

	private void _on_parry_trigger_area_entered(Area2D area)
	{
		GD.Print("Shield: AREA ENTERED: "+ area.Name);
		if (area is ParryArea)
		{
			TempAudioGlobal.Play("hit_critPLACEHOLDER.mp3", this);
		}
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
		var depth = collision.GetRemainder().Length();
		var pushback = normal * depth * 50;  //tweak multiplier
		playerRef.Push(pushback);
	}

	public void ParryActive()
	{
		ParryOn = true;
		GetNode<ColorRect>("ParryFx").Visible = true;
	}

	public void ParryOver()
	{
		ParryOn = false;
		GetNode<ColorRect>("ParryFx").Visible = false;
	}

	void ShieldOnSword(Sword sw)
	{
		if (ParryOn)
		{
			GD.Print("ACTUALL PARRY");
			TempAudio.Play("ParryPLACEHOLDER.wav", this);
			sw.playerRef.Stun(0.6f);
		}
		else
		{
			sw.playerRef.Stun(0.1f);
		this.playerRef.Stun(0.1f);
		}
		Vector2 pushdir = Vector2.FromAngle(this.GlobalRotation + Rotation);
		GD.Print(pushdir);
		sw.playerRef.Push(pushdir * -300);
		
		//GD.Print("SHIELD ON SWORD CONTACT");
	}

	void ShieldOnShield(Shield sd)
	{
		Vector2 pushdir = Vector2.FromAngle(this.GlobalRotation + Rotation);
		sd.playerRef.Push(pushdir * -100);
		//GD.Print("SHIELD ON SHIELD CONTACT");
	}
}
