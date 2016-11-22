using System.Collections.Generic;
using UnityEngine;

public class MapManager
{
	public TileMap<Cell> Map { get { return map; } }

	private TileMap<Cell> map;
	private Dictionary<int, GameObject> entities;

	public MapManager()
	{
		entities = new Dictionary<int, GameObject>();
	}

	public Cell this[int x, int y]
	{
		get { return map[x, y]; }
	}

	public void Initialize(TileMap<TileInfo> blueprint)
	{
		map = blueprint.Convert((TileInfo info) =>
		{
			Cell result = new Cell(info);
			return result;
		});

		entities.Clear();
	}

	public bool IsCellFree(int2 cellPosition)
	{
		bool result = false;
		Cell cell = map[cellPosition.x, cellPosition.y];

		if (cell != null && cell.Entity == null && cell.Prop == null)
		{
			result = true;
		}

		return result;
	}

	public bool Spawn(GameObject entity, int2 position)
	{
		bool result = false;

		CellTransform cellTransform = entity.GetComponent<CellTransform>();

		if (cellTransform == null)
		{
			throw new System.Exception("Every entity must contain a CellTransform component in order to be handled by a MapManager");
		}

		if (!entities.ContainsKey(entity.GetInstanceID()))
		{
			Cell spawnPoint = map[position.x, position.y];

			if (spawnPoint != null && spawnPoint.Entity == null)
			{
				spawnPoint.Entity = entity;
				entities.Add(entity.GetInstanceID(), entity);
				cellTransform.Position = position;
				result = true;
			}
		}

		return result;
	}

	public bool MoveTo(GameObject entity, int2 position)
	{
		bool result = false;

		if (entities.ContainsKey(entity.GetInstanceID()))
		{
			CellTransform cellTransform = entity.GetComponent<CellTransform>();
			Cell movePoint = map[position.x, position.y];

			if (movePoint != null && movePoint.Entity == null)
			{
				movePoint.Entity = entity;
				cellTransform.Position = position;
				result = true;
			}
		}

		return result;
	}
}
