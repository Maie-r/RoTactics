using Godot;
using System;
using System.Collections.Generic;

public partial class Shield : StaticBody2D
{
	public Player playerRef;
	bool ParryOn = false;

	HashSet<Node2D> Collisioned = new();
	private void _on_parry_trigger_body_entered(Node2D Body)
	{
		GD.Print("Shield: BODY ENTERED: "+ Body.Name);
		Collisioned.Add(Body);
		/*if (Body is Sword sw)
			ShieldOnSword(sw);
		else if (Body is Shield sd)
			ShieldOnShield(sd);//*/

	}

	private void _on_parry_trigger_body_exited(Node2D Body)
	{
		GD.Print("Shield: BODY EXITED: "+ Body.Name);
		Collisioned.Remove(Body);/*
		if (Body is Sword sw)
			ShieldOnSword(sw);
		else if (Body is Shield sd)
			ShieldOnShield(sd);*/

	}

	private void _on_parry_trigger_area_entered(Area2D area)
	{
		GD.Print("Shield: AREA ENTERED: "+ area.Name);
		if (area is ParryArea)
		{
			//TempAudio.PlayRandomPitch("Parried.mp3", 0.1f, this);
		}
	}

	public override void _Process(double delta)
	{
		foreach(var collision in Collisioned)
		{
			if (collision is Sword sw)
				ShieldOnSword(sw);
			else if (collision is Shield sd)
				ShieldOnShield(sd);
		}
		var target = playerRef.GlobalPosition;
		var motion = target - this.GlobalPosition;
		//var collision = MoveAndCollide(motion);
		//if (collision != null)
			//HandleCollision(collision);
		//MoveAndSlide();
		//CheckCollision((float)delta);

	}

	/*void CheckCollision(float delta) //never called
	{
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			GD.Print(GetSlideCollision(i).GetType());
			var collision = GetSlideCollision(i);
			if (collision.GetCollider() is RigidBody2D rb)
			{
				
				var push = (playerRef.Velocity.Length() * delta) + 1;
				rb.ApplyCentralImpulse(-collision.GetNormal() * push);
			}
			else if (collision.GetCollider() is Shield sh)
			{
				GD.Print("PUSHING SHIELD");
				var push = (playerRef.Velocity.Length() * delta) + 1;
				sh.playerRef.Push(-collision.GetNormal() * push);
			}
		}
	}*/

	void HandleCollision(KinematicCollision2D collision)
	{
		GD.Print(collision);
		var normal = collision.GetNormal();
		var depth = collision.GetRemainder().Length();
		var pushback = normal * depth * 100;  //tweak multiplier
		playerRef.Push(pushback);
	}

	public void ParryActive()
	{
		ParryOn = true;
		GetNode<ColorRect>("ParryFx").Visible = true;
		TempAudio.PlayRandomPitch("ParryStart.mp3", 0.1f, this);
	}

	public void ParryOver()
	{
		ParryOn = false;
		GetNode<ColorRect>("ParryFx").Visible = false;
		TempAudio.PlayRandomPitch("ParryEnd.mp3", 0.1f, this);
	}

	void ShieldOnSword(Sword sw)
	{
		if (ParryOn)
		{
			GD.Print("ACTUALL PARRY");
			
			var dir = Mathf.Sign(sw.AngularVelocity);
			if (dir != 0)
			{
				sw.playerRef.PushRotation(5 * dir);
				sw.playerRef.Stun(1);
				TempAudio.PlayRandomPitch("Parried.mp3", 0.2f, this);
			}
			else
				sw.playerRef.Stun(0.1f);
		}
		else
		{
			sw.playerRef.Stun(0.1f);
		}
		Vector2 pushdir = Vector2.FromAngle(this.GlobalRotation + Rotation);
		//GD.Print(pushdir);
		sw.playerRef.Push(pushdir * -500);
		
		//GD.Print("SHIELD ON SWORD CONTACT");//*/
		TempAudio.PlayRandomPitch("HitShield.mp3", 0.1f, this);
	}

	void ShieldOnShield(Shield sd)
	{
		Vector2 pushdir = Vector2.FromAngle(this.GlobalRotation - sd.GlobalRotation);
		sd.playerRef.Push(pushdir * 200);
		GD.Print("SHIELD ON SHIELD CONTACT");//*/
		//TempAudio.PlayRandomPitch("HitShield.mp3", 0.1f, this);
	}

	public void SetCollisions(int ID)
	{
		if (ID > 0)
		{
			uint CollisionValue = (uint)Math.Pow(2, ID + 3);  //Player/Shield line
			uint CollisionValue2 = (uint)Math.Pow(2, ID + 7); //Sword Line
			uint CollisionValue3 = (uint)Math.Pow(2, ID + 11);//Parry Line
			this.CollisionLayer = CollisionValue;
			this.CollisionMask -= CollisionValue2;// + CollisionValue3;
			var parry = this.GetNode<Area2D>("ParryTrigger");
			parry.CollisionLayer = CollisionValue3;
			parry.CollisionMask -= CollisionLayer + CollisionValue2;
		}
	}
}
