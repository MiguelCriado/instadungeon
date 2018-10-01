using InstaDungeon.Components;
using InstaDungeon.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace InstaDungeon
{
	public enum EntityTypeInMap
	{
		Actor,
		Prop,
		Item
	}

	public class MapManager : Manager
	{
		public EventSystem Events { get { return events; } }
		public TileMap<Cell> Map { get { return map; } }

		private EventSystem events;
		private EntityManager entityManager;
		private TileMap<Cell> map;
		private AStarSearch<int2, int> pathfinder;
		private Dictionary<uint, Entity> actors;
		private Dictionary<uint, Entity> props;
		private Dictionary<uint, Entity> items;
		private TileMapWeightedGraph defaultGraph;
		private TileMapIgnoreActorsWeightedGraph ignoreActorsGraph;

		private List<Entity> cachedActors;
		private List<Entity> cachedProps;
		private List<Entity> cachedItems;
		private bool actorsDirty;
		private bool propsDirty;
		private bool itemsDirty;

		public MapManager() : base()
		{
			events = new EventSystem();
			entityManager = Locator.Get<EntityManager>();
			actors = new Dictionary<uint, Entity>();
			props = new Dictionary<uint, Entity>();
			items = new Dictionary<uint, Entity>();

			cachedActors = new List<Entity>();
			cachedProps = new List<Entity>();
			cachedItems = new List<Entity>();
			actorsDirty = true;
			propsDirty = true;
			itemsDirty = true;
		}

		public Cell this[int x, int y]
		{
			get { return map[x, y]; }
		}

		public Cell this[int2 position]
		{
			get { return map[position]; }
		}

		public void Initialize(TileMap<Cell> map)
		{
			Clear();

			this.map = map;

			defaultGraph = new TileMapWeightedGraph(map);
			pathfinder = new AStarSearch<int2, int>(defaultGraph, new ManhattanDistanceHeuristic());
			ignoreActorsGraph = new TileMapIgnoreActorsWeightedGraph(map);
		}

		public void Clear()
		{
			DisposeEntities(actors, (Entity entity, int2 position) => RemoveActor(entity, position));
			DisposeEntities(props, (Entity entity, int2 position) => RemoveProp(entity, position));
			DisposeEntities(items, (Entity entity, int2 position) => RemoveItem(entity, position));

			actors.Clear();
			props.Clear();
			items.Clear();
		}

		#region [Common]

		public bool Contains(Entity entity)
		{
			return actors.ContainsKey(entity.Guid) || props.ContainsKey(entity.Guid) || items.ContainsKey(entity.Guid);
		}

		#endregion

		#region [Actors]

		public List<Entity> GetActors()
		{
			if (actorsDirty)
			{
				cachedActors.Clear();
				var enumerator = actors.GetEnumerator();

				while (enumerator.MoveNext())
				{
					cachedActors.Add(enumerator.Current.Value);
				}

				actorsDirty = false;
			}

			return cachedActors;
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

		public bool AddActor(Entity actor, int2 cellPosition)
		{
			bool result = false;
			Cell cell = map[cellPosition];

			if (actors.ContainsKey(actor.Guid))
			{
				Debug.LogError(string.Format("Cannot add actor '{0}'. Actor already in map at position {1}.", actor.name, actor.CellTransform.Position), actor);
			}
			else if (cell == null)
			{
				Debug.LogError(string.Format("Cannot add actor '{0}'. CellPosition ({1}) is not valid.", actor, cellPosition), actor);
			}
			else if (cell.Actor != null)
			{
				Debug.LogError(string.Format("Cannot add actor '{0}'. CellPosition ({1}) already occupied by actor '{2}'", actor.name, cellPosition, cell.Actor.name), actor);
			}
			else
			{
				cell.Actor = actor;
				actors.Add(actor.Guid, actor);
				actor.CellTransform.MoveTo(cellPosition);
				actor.Events.TriggerEvent(new EntityAddToMapEvent(actor, cellPosition));
				events.TriggerEvent(new ActorAddedToMapEvent(actor, cellPosition));
				result = true;
				actorsDirty = true;
			}

			return result;
		}

		public bool RemoveActor(Entity actor, int2 cellPosition)
		{
			bool result = false;
			Cell cell = map[cellPosition];

			if (!actors.ContainsKey(actor.Guid))
			{
				Debug.LogError(string.Format("Cannot remove actor '{0}'. Actor not found in map.", actor.name), actor);
			}
			else if (cell == null)
			{
				Debug.LogError(string.Format("Cannot remove actor '{0}'. CellPosition ({1}) is not valid.", actor, cellPosition), actor);
			}
			else if (cell.Actor != actor)
			{
				Debug.LogError(string.Format("Cannot remove actor '{0}' from position '{1}'. The requested actor is not there.", actor, cellPosition), actor);
			}
			else
			{
				cell.Actor = null;
				actors.Remove(actor.Guid);
				events.TriggerEvent(new ActorRemovedFromMapEvent(actor, cellPosition));
				actor.Events.TriggerEvent(new EntityRemoveFromMapEvent(actor, cellPosition));
				result = true;
				actorsDirty = true;
			}

			return result;
		}

		public bool MoveActorTo(Entity actor, int2 position)
		{
			bool result = false;

			if (actors.ContainsKey(actor.Guid))
			{
				int2 currentEntityPosition = actor.CellTransform.Position;
				Cell currentPoint = map[currentEntityPosition.x, currentEntityPosition.y];
				Cell movePoint = map[position.x, position.y];

				if (movePoint != null && movePoint.TileInfo.Walkable && movePoint.Actor == null
					&& currentPoint != null && currentPoint.Actor == actor)
				{
					currentPoint.Actor = null;
					movePoint.Actor = actor;
					result = true;
				}
			}

			return result;
		}

		#endregion

		#region [Props]

		public List<Entity> GetProps()
		{
			if (propsDirty)
			{
				cachedProps.Clear();
				var enumerator = props.GetEnumerator();

				while (enumerator.MoveNext())
				{
					cachedProps.Add(enumerator.Current.Value);
				}

				propsDirty = false;
			}

			return cachedProps;
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
				prop.Events.TriggerEvent(new EntityAddToMapEvent(prop, cellPosition));
				result = true;
			}

			return result;
		}

		public bool RemoveProp(Entity prop, int2 cellPosition)
		{
			bool result = false;
			Cell cell = map[cellPosition];

			if (!props.ContainsKey(prop.Guid))
			{
				Debug.LogError(string.Format("Cannot remove prop '{0}'. Prop not found in map.", prop.name), prop);
			}
			else if (cell == null)
			{
				Debug.LogError(string.Format("Cannot remove prop '{0}'. CellPosition ({1}) is not valid.", prop, cellPosition), prop);
			}
			else if (cell.Prop != prop)
			{
				Debug.LogError(string.Format("Cannot remove prop '{0}' from position '{1}'. The requested prop is not there.", prop, cellPosition), prop);
			}
			else
			{
				cell.Prop = null;
				props.Remove(prop.Guid);
				events.TriggerEvent(new PropRemovedFromMapEvent(prop, cellPosition));
				prop.Events.TriggerEvent(new EntityRemoveFromMapEvent(prop, cellPosition));
				result = true;
				propsDirty = true;
			}

			return result;
		}

		#endregion

		#region [Items]

		public List<Entity> GetItems()
		{
			if (itemsDirty)
			{
				cachedItems.Clear();
				var enumerator = items.GetEnumerator();

				while (enumerator.MoveNext())
				{
					cachedItems.Add(enumerator.Current.Value);
				}

				itemsDirty = false;
			}

			return cachedItems;
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
				item.Events.TriggerEvent(new EntityAddToMapEvent(item, cellPosition));
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
				item.Events.TriggerEvent(new EntityRemoveFromMapEvent(item, cellPosition));
				result = true;
			}

			return result;
		}

		#endregion

		#region [Pathfinding]

		public int2[] GetPath(int2 start, int2 goal)
		{
			defaultGraph.SetGoal(goal);
			return pathfinder.Search(start, goal);
		}

		public int2[] GetPathIgnoringActors(int2 start, int2 goal)
		{
			ignoreActorsGraph.SetGoal(goal);
			return pathfinder.Search(start, goal, ignoreActorsGraph);
		}

		#endregion

		#region [Helpers]

		private void DisposeEntities(Dictionary<uint, Entity> entitySet, UnityAction<Entity, int2> removeEntityCallback)
		{
			List<Entity> entityList = new List<Entity>(entitySet.Values);

			for (int i = 0; i < entityList.Count; i++)
			{
				Entity entity = entityList[i];
				removeEntityCallback(entity, entity.CellTransform.Position);
				entityManager.Recycle(entity.Guid);
			}
		}

		#endregion
	}
}
