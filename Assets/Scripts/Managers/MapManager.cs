using InstaDungeon.Commands;
using System.Collections.Generic;
using UnityEngine;

namespace InstaDungeon
{
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

		public bool Spawn(MoveActorCommand spawnCommand)
		{
			bool result = false;

			GameObject entity = spawnCommand.ActorTransform.gameObject;

			if (!entities.ContainsKey(entity.GetInstanceID()))
			{
				Cell spawnPoint = map[spawnCommand.Position.x, spawnCommand.Position.y];

				if (spawnPoint != null && spawnPoint.TileInfo.Walkable && spawnPoint.Entity == null)
				{
					spawnPoint.Entity = spawnCommand.ActorTransform.gameObject;
					entities.Add(entity.GetInstanceID(), entity);
					spawnCommand.Execute();
					result = true;
				}
			}

			return result;
		}

		public bool MoveTo(MoveActorCommand moveCommand)
		{
			bool result = false;

			GameObject entity = moveCommand.ActorTransform.gameObject;

			if (entities.ContainsKey(entity.GetInstanceID()))
			{
				Cell currentPoint = map[moveCommand.ActorTransform.Position.x, moveCommand.ActorTransform.Position.y];
				Cell movePoint = map[moveCommand.Position.x, moveCommand.Position.y];

				if (movePoint != null && movePoint.TileInfo.Walkable && movePoint.Entity == null
					&& currentPoint != null && currentPoint.Entity == entity)
				{
					currentPoint.Entity = null;
					movePoint.Entity = entity;
					moveCommand.Execute();
					result = true;
				}
			}

			return result;
		}
	}
}
