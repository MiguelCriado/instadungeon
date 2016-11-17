using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
	private TileMap<Cell> map;
	private Dictionary<int, GameObject> entities;

	protected void Awake()
	{
		entities = new Dictionary<int, GameObject>();
	}

	public void Initialize(TileMap<TileInfo> blueprint)
	{
		map = blueprint.Convert((TileInfo info) =>
		{
			Cell result = new Cell(info);
			return result;
		});
	}

	public bool Spawn(GameObject entity, int2 position)
	{
		bool result = false;

		if (!entities.ContainsKey(entity.GetInstanceID()))
		{
			Cell spawnPoint = map[position.x, position.y];

			if (spawnPoint != null && spawnPoint.Entity == null)
			{
				spawnPoint.Entity = entity;
				entities.Add(entity.GetInstanceID(), entity);
				result = true;
			}
		}

		return result;
	}

	public bool Move(GameObject entity, int2 position)
	{
		bool result = false;

		if (entities.ContainsKey(entity.GetInstanceID()))
		{
			Cell movePoint = map[position.x, position.y];

			if (movePoint != null && movePoint.Entity == null)
			{
				movePoint.Entity = entity;
				result = true;
			}
		}

		return result;
	}
}
