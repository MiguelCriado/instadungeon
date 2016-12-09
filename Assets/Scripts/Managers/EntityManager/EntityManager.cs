using InstaDungeon.Components;
using InstaDungeon.Events;
using System.Collections.Generic;
using UnityEngine;

namespace InstaDungeon
{
	[RequireComponent(typeof(EntityLoader))]
	public class EntityManager : MonoBehaviour
	{
		public uint NextGuid { get { return nextGuid; } }
		public EventSystem Events { get { return events; } }

		private Dictionary<uint, Entity> dynamicEntities;
		private EntityLoader loader;
		private EventSystem events;
		private uint nextGuid;
		private Transform entitiesContainer;

		void Awake()
		{
			nextGuid = 0;
			events = new EventSystem();
			dynamicEntities = new Dictionary<uint, Entity>();
			loader = GetComponent<EntityLoader>();
			GameObject entitiesGO = GameObject.FindGameObjectWithTag("Entities");

			if (entitiesGO == null)
			{
				GameObject world = GameObject.FindGameObjectWithTag("World");

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

		private void SubscribeToEvents(Entity entity)
		{
			entity.Events.AddListener(OnEntityMove, EntityMoveEvent.EVENT_TYPE);
		}

		private void UnsubscribeToEvents(Entity entity)
		{
			entity.Events.RemoveListener(OnEntityMove, EntityMoveEvent.EVENT_TYPE);
		}

		private void OnEntityMove(IEventData eventData)
		{
			events.TriggerEvent(eventData);
		}
	}
}
