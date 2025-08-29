using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Map : Node2D
{
	public List<Node> GetBackgrounds()
	{
		return GetNode<Node2D>("Walls").GetChildren().ToList<Node>();
	}
	public List<Node2D> GetWalls()
	{
		var res = new List<Node2D>();
		foreach (var child in GetNode<Node2D>("Walls").GetChildren())
		{
			res.Add(child as Node2D);
		}
		return res;
	}
	public List<Node2D> GetInteractables()
	{
		var res = new List<Node2D>();
		foreach (var child in GetNode<Node2D>("Interactables").GetChildren())
		{
			res.Add(child as Node2D);
		}
		return res;
	}
	public List<Marker2D> GetSpawns()
	{
		var res = new List<Marker2D>();
		foreach (var child in GetNode<Node2D>("SpawnPoints").GetChildren())
		{
			res.Add(child as Marker2D);
		}
		return res;
	}
}
