using InstaDungeon.Commands;
using InstaDungeon.Components;
using InstaDungeon.Events;
using System.Collections.Generic;

namespace InstaDungeon
{
	public class MapManager : Manager
	{
		public TileMap<Cell> Map { get { return map; } }

		private TileMap<Cell> map;
		private Dictionary<uint, Entity> entities;

		public MapManager() : base()
		{
			entities = new Dictionary<uint, Entity>();
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

		public bool CanCellBeOccupied(int2 cellPosition)
		{
			bool result = false;
			Cell cell = map[cellPosition.x, cellPosition.y];

			if (cell != null && cell.TileInfo.Walkable && cell.Entity == null)
			{
				result = true;
			}

			return result;
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

		public bool Relocate(MoveEntityCommand relocateCommand)
		{
			bool result = false;

			Entity entity = relocateCommand.Entity;

			if (!entities.ContainsKey(entity.Guid))
			{
				Cell spawnPoint = map[relocateCommand.Position.x, relocateCommand.Position.y];

				if (spawnPoint != null && spawnPoint.TileInfo.Walkable && spawnPoint.Entity == null)
				{
					spawnPoint.Entity = relocateCommand.Entity;
					entities.Add(entity.Guid, entity);
					relocateCommand.Execute();
					result = true;

					entity.Events.TriggerEvent(new EntityRelocateEvent(entity.Guid, relocateCommand.LastPosition, relocateCommand.Position));
				}
			}

			return result;
		}

		public bool MoveTo(Entity entity, int2 position)
		{
			bool result = false;

			if (entities.ContainsKey(entity.Guid))
			{
				int2 currentEntityPosition = entity.CellTransform.Position;
				Cell currentPoint = map[currentEntityPosition.x, currentEntityPosition.y];
				Cell movePoint = map[position.x, position.y];

				if (movePoint != null && movePoint.TileInfo.Walkable && movePoint.Entity == null
					&& currentPoint != null && currentPoint.Entity == entity)
				{
					currentPoint.Entity = null;
					movePoint.Entity = entity;
					result = true;
				}
			}

			return result;
		}
	}
}
