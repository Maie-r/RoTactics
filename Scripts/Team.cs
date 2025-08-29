using Godot;
using System;
using System.Collections.Generic;

public class Team
{
	public static List<Team> Teams = new List<Team>();
	public static Team GetTeam(int i)
	{
		if (i >= 0 && i < Teams.Count)
		{
			return Teams[i];
		}
		return null;
	}

	public static void ClearTeams()
	{
		Teams.Clear();
	}
	public string Color;
	public int ID;
	public int Score = 0;

	/// <summary>
	/// Applies color based on Team color
	/// </summary>
	public void ApplyColor(Node2D Body)
	{
		try
		{
			Body.Material = Body.Material.Duplicate() as ShaderMaterial;
			if (Body.Material is ShaderMaterial sm)
			{
				Color color = new Color(Color);
				sm.SetShaderParameter("Colorize", color);
			}
		}
		catch
		{
			GD.PushWarning("Couldn't apply color to " + Body.Name);
		}
	}

	/// <summary>
	/// Applies default color based on ID
	/// </summary>
	public static void ApplyColor(Player Body)
	{
		try
		{
			Body.Material = Body.Material.Duplicate() as ShaderMaterial;
			Color color;
			switch (Body.ID)
			{
				case 1:
					color = new Color("red");
					break;
				case 2:
					color = new Color("blue");
					break;
				case 3:
					color = new Color("green");
					break;
				case 4:
					color = new Color("yellow");
					break;
				default:
					color = new Color("white");
					break;
			}
			var sm = (ShaderMaterial)Body.Material;
			sm.SetShaderParameter("Colorize", color);
		}
		catch
		{
			GD.PushWarning("Couldn't apply color to " + Body.Name);
		}
	}
}
