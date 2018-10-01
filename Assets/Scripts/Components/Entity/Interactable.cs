using InstaDungeon.Events;
using System.Collections.Generic;
using UnityEngine;

namespace InstaDungeon.Components
{
	[RequireComponent(typeof(Entity))]
	public class Interactable : MonoBehaviour
	{
		[SerializeField] protected Interaction interaction;

		private EntityManager entityManager;
		private MapManager mapManager;
		private Entity entity;

		private void Awake()
		{
			entityManager = Locator.Get<EntityManager>();
			mapManager = Locator.Get<MapManager>();
			entity = GetComponent<Entity>();
		}

		private void OnEnable()
		{
			entityManager.Events.AddListener(OnEntityGrantTurn, EntityGrantTurnEvent.EVENT_TYPE);
			entityManager.Events.AddListener(OnEntityRevokeTurn, EntityRevokeTurnEvent.EVENT_TYPE);
		}

		private void OnDisable()
		{
			entityManager.Events.RemoveListener(OnEntityGrantTurn, EntityGrantTurnEvent.EVENT_TYPE);
			entityManager.Events.RemoveListener(OnEntityRevokeTurn, EntityRevokeTurnEvent.EVENT_TYPE);
		}

		#region [Public API]

		public bool IsValidInteraction(Entity actor)
		{
			return interaction.IsValidInteraction(actor, entity);
		}

		public void Interact(Entity actor, TurnToken token)
		{
			token.BufferAction(interaction.Interact(actor, entity));
		}

		#endregion

		#region [Event Reactions]

		private void OnInteractableAddToMap(IEventData eventData)
		{
			CheckForPotentialInteraction();
			// TODO register event handler
		}

		private void OnEntityGrantTurn(IEventData eventData)
		{
			EntityGrantTurnEvent entityEvent = eventData as EntityGrantTurnEvent;

			if (entityEvent.Entity != entity)
			{
				entityEvent.Entity.Events.AddListener(OnEntityStartMovement, EntityStartMovementEvent.EVENT_TYPE);
				entityEvent.Entity.Events.AddListener(OnEntityFinishMovement, EntityFinishMovementEvent.EVENT_TYPE);
				entityEvent.Entity.Events.AddListener(OnEntityAddToMapEvent, EntityAddToMapEvent.EVENT_TYPE);
				entityEvent.Entity.Events.AddListener(OnEntityRemoveFromMapEvent, EntityRemoveFromMapEvent.EVENT_TYPE);
			}
		}

		private void OnEntityRevokeTurn(IEventData eventData)
		{
			EntityRevokeTurnEvent entityEvent = eventData as EntityRevokeTurnEvent;

			if (entityEvent.Entity != entity)
			{
				entityEvent.Entity.Events.RemoveListener(OnEntityStartMovement, EntityStartMovementEvent.EVENT_TYPE);
				entityEvent.Entity.Events.RemoveListener(OnEntityFinishMovement, EntityFinishMovementEvent.EVENT_TYPE);
				entityEvent.Entity.Events.RemoveListener(OnEntityAddToMapEvent, EntityAddToMapEvent.EVENT_TYPE);
				entityEvent.Entity.Events.RemoveListener(OnEntityRemoveFromMapEvent, EntityRemoveFromMapEvent.EVENT_TYPE);
			}
		}

		private void OnEntityAddToMapEvent(IEventData eventData)
		{
			if (mapManager.Contains(entity))
			{
				EntityAddToMapEvent entityEvent = eventData as EntityAddToMapEvent;

				if (IsEntityInRange(entityEvent.Position, entity.CellTransform.Position))
				{
					interaction.EntityEntersRange(entityEvent.Entity, entity);
				}
			}
		}

		private void OnEntityRemoveFromMapEvent(IEventData eventData)
		{
			if (mapManager.Contains(entity))
			{
				EntityRemoveFromMapEvent entityEvent = eventData as EntityRemoveFromMapEvent;

				if (IsEntityInRange(entityEvent.Position, entity.CellTransform.Position))
				{
					interaction.EntityExitsRange(entityEvent.Entity, entity);
				}
			}
		}

		private void OnEntityStartMovement(IEventData eventData)
		{
			if (mapManager.Contains(entity))
			{
				EntityStartMovementEvent entityEvent = eventData as EntityStartMovementEvent;

				if (IsEntityInRange(entityEvent.NextPosition, entity.CellTransform.Position))
				{
					Entity activeEntity = entityManager.Get(entityEvent.EntityId);
					interaction.EntityEntersRange(activeEntity, entity);
				}
			}
		}

		private void OnEntityFinishMovement(IEventData eventData)
		{
			if (mapManager.Contains(entity))
			{
				EntityFinishMovementEvent entityEvent = eventData as EntityFinishMovementEvent;

				if (IsEntityInRange(entityEvent.PreviousPosition, entity.CellTransform.Position) == true
					&& IsEntityInRange(entityEvent.CurrentPosition, entity.CellTransform.Position) == false)
				{
					Entity activeEntity = entityManager.Get(entityEvent.EntityId);
					interaction.EntityExitsRange(activeEntity, entity);
				}
			}
		}

		#endregion

		#region [Helpers]

		private bool IsEntityInRange(int2 entityPosition, int2 interactablePosition)
		{
			int manhattanDistance = int2.ManhattanDistance(entityPosition, interactablePosition);

			return manhattanDistance >= interaction.MinRange && manhattanDistance <= interaction.MaxRange;
		}

		private void CheckForPotentialInteraction()
		{
			List<Entity> actors = mapManager.GetActors();
			var enumerator = actors.GetEnumerator();

			while (enumerator.MoveNext())
			{
				int2 position = enumerator.Current.CellTransform.Position;

				if (IsEntityInRange(position, entity.CellTransform.Position))
				{
					interaction.EntityEntersRange(enumerator.Current, entity);
				}
			}
		}

		#endregion
	}
}
