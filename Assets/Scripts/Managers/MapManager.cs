using InstaDungeon.Commands;
using InstaDungeon.Components;
using InstaDungeon.Events;
using System.Collections.Generic;
using UnityEngine;

namespace InstaDungeon
{
	public class MapManager : Manager
	{
		public TileMap<Cell> Map { get { return map; } }

		private EntityManager entityManager;
		private TileMap<Cell> map;
		private Dictionary<uint, Entity> actors;
		private Dictionary<uint, Entity> props;
		private Dictionary<uint, Entity> items;

		public MapManager() : base()
		{
			entityManager = Locator.Get<EntityManager>();
			actors = new Dictionary<uint, Entity>();
			props = new Dictionary<uint, Entity>();
			items = new Dictionary<uint, Entity>();
		}

		public Cell this[int x, int y]
		{
			get { return map[x, y]; }
		}

		public void Initialize(TileMap<Cell> map)
		{
			this.map = map;

			// DisposeEntities(actors);
			DisposeEntities(props);
			DisposeEntities(items);

			actors.Clear();
			props.Clear();
			items.Clear();
		}

		public bool AddProp(Entity prop, int2 cellPosition)
		{
			bool result = false;
			Cell cell = map[cellPosition];

			if (props.ContainsKey(prop.Guid))
			{
				Debug.LogError(string.Format("Cannot add prop '{0}'. Prop already in map at position {1}.", prop.name, prop.CellTransform.Position), prop);
			}
			else if (cell == null)
			{
				Debug.LogError(string.Format("Cannot add prop '{0}'. CellPosition ({1}) is not valid.", prop, cellPosition), prop);
			}
			else if (cell.Prop != null)
			{
				Debug.LogError(string.Format("Cannot add prop '{0}'. Position already occupied by '{1}' (Guid = {2})", prop.name, cell.Prop.name, cell.Prop.Guid), cell.Prop);
			}
			else
			{
				cell.Prop = prop;
				props.Add(prop.Guid, prop);
				prop.CellTransform.MoveTo(cellPosition);
				prop.Events.TriggerEvent(new EntityAddToMapEvent(prop));
				result = true;
			}

			return result;
		}

		public bool AddItem(Entity item, int2 cellPosition)
		{
			bool result = false;
			Cell cell = map[cellPosition];

			if (items.ContainsKey(item.Guid))
			{
				Debug.LogError(string.Format("Cannot add item '{0}'. Item already in map at position {1}.", item.name, item.CellTransform.Position), item);
			}
			else if (cell == null)
			{
				Debug.LogError(string.Format("Cannot add item '{0}'. CellPosition ({1}) is not valid.", item, cellPosition), item);
			}
			else
			{
				cell.Items.Add(item);
				items.Add(item.Guid, item);
				item.CellTransform.MoveTo(cellPosition);
				item.Events.TriggerEvent(new EntityAddToMapEvent(item));
				result = true;
			}

			return result;
		}

		public bool RemoveItem(Entity item, int2 cellPosition)
		{
			bool result = false;
			Cell cell = map[cellPosition];

			if (!items.ContainsKey(item.Guid))
			{
				Debug.LogError(string.Format("Cannot remove item '{0}'. Item not found in map.", item.name), item);
			}
			else if (cell == null)
			{
				Debug.LogError(string.Format("Cannot remove item '{0}'. CellPosition ({1}) is not valid.", item, cellPosition), item);
			}
			else if (!cell.Items.Contains(item))
			{
				Debug.LogError(string.Format("Cannot remove item '{0}' from position '{1}'. The requested item is not there.", item, cellPosition), item);
			}
			else
			{
				cell.Items.Remove(item);
				items.Remove(item.Guid);
				result = true;
			}

			return result;
		}

		public bool CanCellBeOccupiedByActor(int2 cellPosition)
		{
			bool result = false;
			Cell cell = map[cellPosition.x, cellPosition.y];

			if (cell != null && cell.IsWalkable())
			{
				result = true;
			}

			return result;
		}

		public bool IsCellFreeForActor(int2 cellPosition)
		{
			bool result = false;
			Cell cell = map[cellPosition.x, cellPosition.y];

			if (cell != null && cell.Actor == null && cell.Prop == null)
			{
				result = true;
			}

			return result;
		}

		public bool RelocateActor(MoveEntityCommand relocateCommand)
		{
			bool result = false;

			Entity entity = relocateCommand.Entity;

			if (!actors.ContainsKey(entity.Guid))
			{
				Cell spawnPoint = map[relocateCommand.Position.x, relocateCommand.Position.y];

				if (spawnPoint != null && spawnPoint.TileInfo.Walkable && spawnPoint.Actor == null)
				{
					spawnPoint.Actor = relocateCommand.Entity;
					actors.Add(entity.Guid, entity);
					relocateCommand.Execute();
					result = true;

					entity.Events.TriggerEvent(new EntityRelocateEvent(entity.Guid, relocateCommand.LastPosition, relocateCommand.Position));
				}
			}

			return result;
		}

		public bool MoveActorTo(Entity entity, int2 position)
		{
			bool result = false;

			if (actors.ContainsKey(entity.Guid))
			{
				int2 currentEntityPosition = entity.CellTransform.Position;
				Cell currentPoint = map[currentEntityPosition.x, currentEntityPosition.y];
				Cell movePoint = map[position.x, position.y];

				if (movePoint != null && movePoint.TileInfo.Walkable && movePoint.Actor == null
					&& currentPoint != null && currentPoint.Actor == entity)
				{
					currentPoint.Actor = null;
					movePoint.Actor = entity;
					result = true;
				}
			}

			return result;
		}

		private void DisposeEntities(Dictionary<uint, Entity> entitySet)
		{
			var enumerator = entitySet.GetEnumerator();

			while (enumerator.MoveNext())
			{
				entityManager.Recycle(enumerator.Current.Key);
			}
		}
	}
}
