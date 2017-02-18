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
			CreateEntitiesContainer();
		}

		public Entity Spawn(string entityType)
		{
			Entity result = loader.Spawn(entityType, entitiesContainer);

			if (result != null)
			{
				result.Init(nextGuid++);
				dynamicEntities.Add(result.Guid, result);
				SubscribeToEvents(result);
			}

			return result;
		}

		public bool Recycle(uint entityGuid)
		{
			Entity entityToRecycle;

			if (dynamicEntities.TryGetValue(entityGuid, out entityToRecycle))
			{
				UnsubscribeToEvents(entityToRecycle);
				loader.Dispose(entityToRecycle);
				return true;
			}
			else
			{
				return false;
			}
		}

		protected void CreateEntitiesContainer()
		{
			GameObject entitiesGO = GameObject.FindGameObjectWithTag("Entities");

			if (entitiesGO == null)
			{
				GameObject world = GameObject.FindGameObjectWithTag("World");

				if (world == null)
				{
					world = GameObject.Find("World");
				}

				if (world == null)
				{
					world = new GameObject("World");
					world.tag = "World";
				}

				entitiesGO = new GameObject("Entities");
				entitiesGO.tag = "Entities";
				entitiesGO.transform.SetParent(world.transform);
			}

			entitiesContainer = entitiesGO.transform;
		}

		protected override void OnSceneUnLoaded(Scene scene)
		{
			base.OnSceneUnLoaded(scene);
			CreateEntitiesContainer();
		}

		protected void SubscribeToEvents(Entity entity)
		{
			entity.Events.AddListener(OnEntityMove, EntityMoveEvent.EVENT_TYPE);
		}

		protected void UnsubscribeToEvents(Entity entity)
		{
			entity.Events.RemoveListener(OnEntityMove, EntityMoveEvent.EVENT_TYPE);
		}

		protected void OnEntityMove(IEventData eventData)
		{
			events.TriggerEvent(eventData);
		}
	}
}
