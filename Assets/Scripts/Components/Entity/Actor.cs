using InstaDungeon.Actions;
using InstaDungeon.Models;
using System;
using UnityEngine;

namespace InstaDungeon.Components
{
	[RequireComponent(typeof(Entity), typeof(Locomotion), typeof(TurnComponent))]
	public class Actor : MonoBehaviour
	{
		private Entity entity;
		private Locomotion locomotion;
		private TurnComponent turn;
		private MapManager mapManager;
		private EntityManager entityManager;

		private int2 up;
		private int2 right;
		private int2 down;
		private int2 left;

		private void Awake()
		{
			entity = GetComponent<Entity>();
			locomotion = GetComponent<Locomotion>();
			turn = GetComponent<TurnComponent>();
			mapManager = Locator.Get<MapManager>();
			entityManager = Locator.Get<EntityManager>();

			up = new int2(0, 1);
			right = new int2(1, 0);
			down = new int2(0, -1);
			left = new int2(-1, 0);
		}

		public void Up()
		{
			ProcessMoveCommand(up);
		}

		public void Right()
		{
			ProcessMoveCommand(right);
		}

		public void Down()
		{
			ProcessMoveCommand(down);
		}

		public void Left()
		{
			ProcessMoveCommand(left);
		}

		public void InteractWithCurrentTile()
		{
			if (turn.EntityCanAct)
			{
				int2 actionPosition = entity.CellTransform.Position;
				Cell actionCell = mapManager.Map[actionPosition];

				if (actionCell != null)
				{
					Interactable interactable = FindValidInteractable(actionCell, entity);

					if (interactable != null)
					{
						interactable.Interact(entity, turn.Token);
					}
				}
			}
		}

		public void ConsumeItemInBag()
		{
			if (turn.EntityCanAct)
			{
				Inventory inventory = entity.GetComponent<Inventory>();

				if (inventory != null)
				{
					Item item = inventory.GetItem(InventorySlotType.Bag);

					if (item != null && item is IConsumable)
					{
						((IConsumable)item).Consume(entity)
						.Then(() => 
						{
							inventory.RemoveItem(item);
							entityManager.Recycle(item.GetComponent<Entity>().Guid);
							turn.Token.OnActionFinished.Invoke();
						})
						.Catch((Exception e) =>
						{
							Debug.Log(e.Message);
						});
					}
				}
			}
		}

		public void PassTurn()
		{
			if (turn.EntityCanAct)
			{
				turn.Token.BufferAction(new PassTurnAction());
			}
		}

		private void ProcessMoveCommand(int2 actionDirection)
		{
			if (turn.EntityCanAct)
			{
				int2 actionPosition = entity.CellTransform.Position + actionDirection;
				Cell actionCell = mapManager.Map[actionPosition];

				if (actionCell != null)
				{
					Interactable interactable = FindValidInteractable(actionCell, entity);

					if (interactable != null)
					{
						interactable.Interact(entity, turn.Token);
					}
					else
					{
						TryMove(actionDirection.x, actionDirection.y);
					}
				}
			}
		}

		#region [Fallback Action: Movement]

		private void TryMove(int x, int y)
		{
			if (locomotion.IsValidMovement(x, y))
			{
				locomotion.Move(new int2(x, y), turn.Token);
			}
		}

		private Interactable FindValidInteractable(Cell actionCell, Entity interactor)
		{
			Interactable result = null;
			Interactable interactable = null;

			if (actionCell.Actor != null)
			{
				interactable = actionCell.Actor.GetComponent<Interactable>();

				if (interactable != null && interactable.IsValidInteraction(entity))
				{
					result = interactable;
				}
			}

			if (result == null && actionCell.Prop != null)
			{
				interactable = actionCell.Prop.GetComponent<Interactable>();

				if (interactable != null && interactable.IsValidInteraction(entity))
				{
					result = interactable;
				}
			}

			if (result == null && actionCell.Items.Count > 0)
			{
				int i = 0;

				while (result == null && i < actionCell.Items.Count)
				{
					interactable = actionCell.Items[i].GetComponent<Interactable>();

					if (interactable != null && interactable.IsValidInteraction(entity))
					{
						result = interactable;
					}

					i++;
				}
			}

			return result;
		}

		#endregion
	}
}
