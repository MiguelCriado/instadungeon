using System.Collections.Generic;
using UnityEngine;
using InstaDungeon;

public class Cell
{
	public TileInfo TileInfo { get; set; }
	public GameObject Entity { get { return entity; } set { entity = value; } }
	public GameObject Prop { get { return prop; } set { prop = value; } }
	public List<GameObject> Items { get { return items; } }
	public VisibilityType Visibility { get { return visibility; } }

	private GameObject entity;
	private GameObject prop;
	private List<GameObject> items;
	private VisibilityType visibility;

	public Cell(TileInfo tileInfo)
	{
		TileInfo = tileInfo;
		items = new List<GameObject>();
		visibility = VisibilityType.Obscured;
	}

	public void AddItem(GameObject item)
	{
		items.Add(item);
	}

	public void RemoveItem(GameObject item)
	{
		items.Remove(item);
	}

	public void RefreshVisibility(bool visible)
	{
		if (visible == true)
		{
			visibility = VisibilityType.Visible;
		}
		else
		{
			if (visibility == VisibilityType.Visible)
			{
				visibility = VisibilityType.PreviouslySeen;
			}
		}
	}

	public bool BreaksLineOfSight()
	{
		return TileInfo.BreaksLineOfSight; // TODO: add entity, prop, and items lineOfSight break
	}
}
