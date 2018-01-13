using InstaDungeon.Components;
using InstaDungeon.Events;
using System.Collections.Generic;
using UnityEngine;

namespace InstaDungeon
{
	public class EntityManager : Manager
	{
		public uint NextGuid { get { return nextGuid; } }
		public EventSystem Events { get { return events; } }
		public List<Entity> Entities { get { return GetCachedEntities(); } }

		private Dictionary<uint, Entity> dynamicEntities;
		private EntityLoader loader;
		private EventSystem events;
		private uint nextGuid;
		private Transform EntitiesContainer
		{
			get
			{
				if (entitiesContainer == null)
				{
					entitiesContainer = GetSceneContainer("World", "Entities");
				}

				return entitiesContainer;
			}
		}
		private Transform entitiesContainer;
		private bool dirty;
		private List<Entity> cachedEntities;

		public EntityManager() : base()
		{
			nextGuid = 0;
			events = new EventSystem();
			dynamicEntities = new Dictionary<uint, Entity>();
			loader = new EntityLoader();
			cachedEntities = new List<Entity>();
			dirty = true;
		}

		public Entity Spawn(string entityType)
		{
			Entity result = loader.Spawn(entityType, EntitiesContainer);

			if (result != null)
			{
				result.Init(nextGuid++);
				dynamicEntities.Add(result.Guid, result);
				SubscribeToEvents(result);
				dirty = true;

				events.TriggerEvent(new EntitySpawnEvent(result.Guid));
			}

			return result;
		}

		public bool Recycle(uint entityGuid)
		{
			Entity entityToRecycle;

			if (dynamicEntities.TryGetValue(entityGuid, out entityToRecycle))
			{
				dynamicEntities.Remove(entityGuid);
				dirty = true;
				entityToRecycle.Events.TriggerEvent(new EntityDisposeEvent(entityToRecycle));
				UnsubscribeToEvents(entityToRecycle);

				loader.Dispose(entityToRecycle);
				return true;
			}
			else
			{
				return false;
			}
		}

		public Entity Get(uint entityGuid)
		{
			Entity result;
			dynamicEntities.TryGetValue(entityGuid, out result);

			return result;
		}

		#region [Events]

		protected void SubscribeToEvents(Entity entity)
		{
			entity.Events.AddListener(HandleEvent, EntitySpawnEvent.EVENT_TYPE);
			entity.Events.AddListener(HandleEvent, EntityDisposeEvent.EVENT_TYPE);

			entity.Events.AddListener(HandleEvent, EntityAddToMapEvent.EVENT_TYPE);
			entity.Events.AddListener(HandleEvent, EntityRemoveFromMapEvent.EVENT_TYPE);
			entity.Events.AddListener(HandleEvent, EntityStartMovementEvent.EVENT_TYPE);
			entity.Events.AddListener(HandleEvent, EntityFinishMovementEvent.EVENT_TYPE);
			entity.Events.AddListener(HandleEvent, EntityRelocateEvent.EVENT_TYPE);

			entity.Events.AddListener(HandleEvent, DoorOpenEvent.EVENT_TYPE);

			entity.Events.AddListener(HandleEvent, EntityDieEvent.EVENT_TYPE);
			entity.Events.AddListener(HandleEvent, EntityHealthChangeEvent.EVENT_TYPE);

			entity.Events.AddListener(HandleEvent, InventoryItemAddEvent.EVENT_TYPE);
			entity.Events.AddListener(HandleEvent, InventoryItemRemoveEvent.EVENT_TYPE);

			entity.Events.AddListener(HandleEvent, TrapDoorOpenEvent.EVENT_TYPE);

			entity.Events.AddListener(HandleEvent, EntityGrantTurnEvent.EVENT_TYPE);
			entity.Events.AddListener(HandleEvent, EntityRevokeTurnEvent.EVENT_TYPE);
			entity.Events.AddListener(HandleEvent, EntityTurnDoneEvent.EVENT_TYPE);
		}

		protected void UnsubscribeToEvents(Entity entity)
		{
			entity.Events.RemoveListener(HandleEvent, EntitySpawnEvent.EVENT_TYPE);
			entity.Events.RemoveListener(HandleEvent, EntityDisposeEvent.EVENT_TYPE);

			entity.Events.RemoveListener(HandleEvent, EntityAddToMapEvent.EVENT_TYPE);
			entity.Events.RemoveListener(HandleEvent, EntityRemoveFromMapEvent.EVENT_TYPE);
			entity.Events.RemoveListener(HandleEvent, EntityStartMovementEvent.EVENT_TYPE);
			entity.Events.RemoveListener(HandleEvent, EntityFinishMovementEvent.EVENT_TYPE);
			entity.Events.RemoveListener(HandleEvent, EntityRelocateEvent.EVENT_TYPE);

			entity.Events.RemoveListener(HandleEvent, DoorOpenEvent.EVENT_TYPE);

			entity.Events.RemoveListener(HandleEvent, EntityDieEvent.EVENT_TYPE);
			entity.Events.RemoveListener(HandleEvent, EntityHealthChangeEvent.EVENT_TYPE);

			entity.Events.RemoveListener(HandleEvent, InventoryItemAddEvent.EVENT_TYPE);
			entity.Events.RemoveListener(HandleEvent, InventoryItemRemoveEvent.EVENT_TYPE);

			entity.Events.RemoveListener(HandleEvent, TrapDoorOpenEvent.EVENT_TYPE);

			entity.Events.RemoveListener(HandleEvent, EntityGrantTurnEvent.EVENT_TYPE);
			entity.Events.RemoveListener(HandleEvent, EntityRevokeTurnEvent.EVENT_TYPE);
			entity.Events.RemoveListener(HandleEvent, EntityTurnDoneEvent.EVENT_TYPE);
		}

		protected void HandleEvent(IEventData eventData)
		{
			// TODO handle

			BroadcastEvent(eventData);
		}

		protected void BroadcastEvent(IEventData eventData)
		{
			events.TriggerEvent(eventData);
		}

		#endregion

		private List<Entity> GetCachedEntities()
		{
			if (dirty)
			{
				cachedEntities.Clear();

				var enumerator = dynamicEntities.Values.GetEnumerator();

				while (enumerator.MoveNext())
				{
					cachedEntities.Add(enumerator.Current);
				}

				dirty = false;
			}

			return cachedEntities;
		}
	}
}
