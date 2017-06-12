using InstaDungeon.Components;
using InstaDungeon.Events;
using System.Collections.Generic;
using UnityEngine;

namespace InstaDungeon.UI
{
	public class EntitiesHealthManager : MonoBehaviour
	{
		[Header("Prefabs")]
		[SerializeField] private EntityHealthController healthControllerPrefab;

		private EntityManager entityManager;
		private Dictionary<Entity, EntityHealthController> healthBars;

		private void Awake()
		{
			healthBars = new Dictionary<Entity, EntityHealthController>();
		}

		private void Start()
		{
			entityManager = Locator.Get<EntityManager>();
			SubscribeEvents();
			LoadInitialData();
		}

		private void OnEntityEntersMap(IEventData eventData)
		{
			EntityAddToMapEvent entityEvent = eventData as EntityAddToMapEvent;
			ProcessNewEntity(entityEvent.Entity);
		}

		private void OnEntityDisposed(IEventData eventData)
		{
			EntityDisposeEvent entityEvent = eventData as EntityDisposeEvent;

			if (healthBars.ContainsKey(entityEvent.Entity))
			{
				EntityHealthController healthController = healthBars[entityEvent.Entity];
				healthBars.Remove(entityEvent.Entity);
				Destroy(healthController.gameObject);
			}
		}

		private void LoadInitialData()
		{
			MapManager mapManager = Locator.Get<MapManager>();
			List<Entity> actors = mapManager.GetActors();

			for (int i = 0; i < actors.Count; i++)
			{
				ProcessNewEntity(actors[i]);
			}
		}

		private void ProcessNewEntity(Entity entity)
		{
			Health health = entity.GetComponent<Health>();

			if (health != null && health.Entity != GameManager.Player)
			{
				EntityHealthController healthBar = Instantiate(healthControllerPrefab, transform);
				RectTransform rectTransform = healthBar.GetComponent<RectTransform>();
				rectTransform.localScale = Vector3.one;
				healthBar.Initialize(health);
				healthBars.Add(entity, healthBar);
			}
		}

		private void SubscribeEvents()
		{
			entityManager.Events.AddListener(OnEntityEntersMap, EntityAddToMapEvent.EVENT_TYPE);
			entityManager.Events.AddListener(OnEntityDisposed, EntityDisposeEvent.EVENT_TYPE);
		}
	}
}
