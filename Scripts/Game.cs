using Godot;
using System;

public partial class Game : Node2D
{
	[Export (PropertyHint.Range, "1,4,1")]
	public int PlayerCount = 2;
	public Map CurrentMap;

	public override void _Ready()
	{
		if (CurrentMap == null)
		{
			var mapcontainer = GetNode<Node2D>("CurrentMap");
			if (mapcontainer.GetChildCount() <= 0)
			{
				GD.Print("No map found in game, setting default map...");
				var defaultmap = ResourceLoader.Load<PackedScene>("res://Scenes/Maps/Map_Empty.tscn").Instantiate<Node2D>();
				mapcontainer.AddChild(defaultmap);
			}
			CurrentMap = mapcontainer.GetChild(0).GetChild<Map>(0);
			GD.Print("Map Loaded!");
		}
		
		var PPS = ResourceLoader.Load<PackedScene>("res://Scenes/Player.tscn");
		var Spawnpoints = CurrentMap.GetSpawns();
		for (int i = 0; i < PlayerCount; i++)
		{
			Player p = PPS.Instantiate<Player>();
			p.ID = i+1;
			p.TeamID = i+1;
			p.GlobalPosition = Spawnpoints[i].GlobalPosition;
			p.DebugDisplay = true;
			if (i == 1)
				p.InvertedLook = true;
			if (i == 3)
				p.DisableMovement = true;
			//p.NoInteractables = true;
			Team.ApplyColor(p);
			AddChild(p);
		}
	}
}
