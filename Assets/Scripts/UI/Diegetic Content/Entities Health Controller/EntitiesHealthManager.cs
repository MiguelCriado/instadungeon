﻿using InstaDungeon.Components;
using InstaDungeon.Events;
using System.Collections.Generic;
using UnityEngine;

namespace InstaDungeon.UI
{
	public class EntitiesHealthManager : MonoBehaviour
	{
		[Header("Prefabs")]
		[SerializeField] private EntityHealthController healthControllerPrefab;

		private GameManager gameManager;
		private EntityManager entityManager;
		private Dictionary<Entity, EntityHealthController> healthBars;
		private bool isInitialized;
		private bool isSubscribed;

		private void Awake()
		{
			healthBars = new Dictionary<Entity, EntityHealthController>();
		}

		private void OnEnable()
		{
			if (isInitialized)
			{
				SubscribeEvents();
			}
		}

		private void OnDisable()
		{
			UnsubscribeEvents();
		}

		private void Start()
		{
			gameManager = Locator.Get<GameManager>();
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

			isInitialized = true;
		}

		private void ProcessNewEntity(Entity entity)
		{
			Health health = entity.GetComponent<Health>();

			if (health != null && health.Entity != gameManager.Player)
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
			if (isSubscribed == false)
			{
				entityManager = Locator.Get<EntityManager>();
				entityManager.Events.AddListener(OnEntityEntersMap, EntityAddToMapEvent.EVENT_TYPE);
				entityManager.Events.AddListener(OnEntityDisposed, EntityDisposeEvent.EVENT_TYPE);

				isSubscribed = true;
			}
		}

		private void UnsubscribeEvents()
		{
			if (isSubscribed == true)
			{
				entityManager = Locator.Get<EntityManager>();
				entityManager.Events.RemoveListener(OnEntityEntersMap, EntityAddToMapEvent.EVENT_TYPE);
				entityManager.Events.RemoveListener(OnEntityDisposed, EntityDisposeEvent.EVENT_TYPE);

				isSubscribed = false;
			}
		}
	}
}
