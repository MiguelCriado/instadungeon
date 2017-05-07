using InstaDungeon.Actions;
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

			up = new int2(0, 1);
			right = new int2(1, 0);
			down = new int2(0, -1);
			left = new int2(-1, 0);
		}

		public void Up()
		{
			ProcessCommand(up);
		}

		public void Right()
		{
			ProcessCommand(right);
		}

		public void Down()
		{
			ProcessCommand(down);
		}

		public void Left()
		{
			ProcessCommand(left);
		}

		public void PassTurn()
		{
			if (turn.EntityCanAct)
			{
				turn.Token.BufferAction(new PassTurnAction());
			}
		}

		private void ProcessCommand(int2 actionDirection)
		{
			if (turn.EntityCanAct)
			{
				int2 actionPosition = entity.CellTransform.Position + actionDirection;
				Cell actionCell = mapManager.Map[actionPosition];

				if (actionCell != null)
				{
					bool interactionFound = false;
					Interactable interactable;

					if (actionCell.Actor != null)
					{
						interactable = actionCell.Actor.GetComponent<Interactable>();

						if (interactable != null && interactable.IsValidInteraction(entity))
						{
							interactable.Interact(entity, turn.Token);
							interactionFound = true;
						}
					}

					if (!interactionFound && actionCell.Prop != null)
					{
						interactable = actionCell.Prop.GetComponent<Interactable>();

						if (interactable != null && interactable.IsValidInteraction(entity))
						{
							interactable.Interact(entity, turn.Token);
							interactionFound = true;
						}
					}

					if (!interactionFound && actionCell.Items.Count > 0)
					{
						int i = 0;

						while (!interactionFound && i < actionCell.Items.Count)
						{
							interactable = actionCell.Items[i].GetComponent<Interactable>();

							if (interactable != null && interactable.IsValidInteraction(entity))
							{
								interactable.Interact(entity, turn.Token);
								interactionFound = true;
							}

							i++;
						}
					}

					if (!interactionFound)
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

		#endregion
	}
}
