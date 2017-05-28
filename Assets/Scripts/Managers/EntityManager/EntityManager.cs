using InstaDungeon.Components;
using InstaDungeon.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InstaDungeon
{
	public class EntityManager : Manager
	{
		public uint NextGuid { get { return nextGuid; } }
		public EventSystem Events { get { return events; } }

		private Dictionary<uint, Entity> dynamicEntities;
		private EntityLoader loader;
		private EventSystem events;
		private uint nextGuid;
		private Transform entitiesContainer;

		public EntityManager() : base()
		{
			nextGuid = 0;
			events = new EventSystem();
			dynamicEntities = new Dictionary<uint, Entity>();
			loader = new EntityLoader();
			entitiesContainer = GetSceneContainer("World", "Entities");
		}

		public Entity Spawn(string entityType)
		{
			Entity result = loader.Spawn(entityType, entitiesContainer);

			if (result != null)
			{
				result.Init(nextGuid++);
				dynamicEntities.Add(result.Guid, result);
				SubscribeToEvents(result);

				events.TriggerEvent(new EntitySpawnEvent(result.Guid));
			}

			return result;
		}

		public bool Recycle(uint entityGuid)
		{
			Entity entityToRecycle;

			if (dynamicEntities.TryGetValue(entityGuid, out entityToRecycle))
			{
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

		protected override void OnSceneUnLoaded(Scene scene)
		{
			base.OnSceneUnLoaded(scene);
			entitiesContainer = GetSceneContainer("World", "Entities");
		}

		#region [Events]

		protected void SubscribeToEvents(Entity entity)
		{
			entity.Events.AddListener(HandleEvent, EntitySpawnEvent.EVENT_TYPE);
			entity.Events.AddListener(HandleEvent, EntityDisposeEvent.EVENT_TYPE);

			entity.Events.AddListener(HandleEvent, EntityAddToMapEvent.EVENT_TYPE);
			entity.Events.AddListener(HandleEvent, EntityStartMovementEvent.EVENT_TYPE);
			entity.Events.AddListener(HandleEvent, EntityFinishMovementEvent.EVENT_TYPE);
			entity.Events.AddListener(HandleEvent, EntityRelocateEvent.EVENT_TYPE);

			entity.Events.AddListener(HandleEvent, DoorOpenEvent.EVENT_TYPE);
		}

		protected void UnsubscribeToEvents(Entity entity)
		{
			entity.Events.RemoveListener(HandleEvent, EntitySpawnEvent.EVENT_TYPE);
			entity.Events.RemoveListener(HandleEvent, EntityDisposeEvent.EVENT_TYPE);

			entity.Events.RemoveListener(HandleEvent, EntityAddToMapEvent.EVENT_TYPE);
			entity.Events.RemoveListener(HandleEvent, EntityStartMovementEvent.EVENT_TYPE);
			entity.Events.RemoveListener(HandleEvent, EntityFinishMovementEvent.EVENT_TYPE);
			entity.Events.RemoveListener(HandleEvent, EntityRelocateEvent.EVENT_TYPE);

			entity.Events.RemoveListener(HandleEvent, DoorOpenEvent.EVENT_TYPE);
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
	}
}
