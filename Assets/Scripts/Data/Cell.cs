using System.Collections.Generic;
using UnityEngine;

public class Cell
{
	public TileInfo TileInfo { get; set; }
	public GameObject Entity { get { return entity; } set { entity = value; } }
	public GameObject Prop { get { return prop; } set { prop = value; } }
	public List<GameObject> Items { get { return items; } }

	private GameObject entity;
	private GameObject prop;
	private List<GameObject> items;

	public Cell(TileInfo tileInfo)
	{
		TileInfo = tileInfo;
		items = new List<GameObject>();
	}

	public void AddItem(GameObject item)
	{
		items.Add(item);
	}

	public void RemoveItem(GameObject item)
	{
		items.Remove(item);
	}
}
