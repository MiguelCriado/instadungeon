using InstaDungeon.Events;
using System.Collections.Generic;
using UnityEngine;

namespace InstaDungeon.Components
{
	[RequireComponent(typeof(Entity), typeof(Inventory))]
	public class ItemAutoPicker : MonoBehaviour
	{
		private Entity entity;
		private Inventory inventory;
		private MapManager mapManager;

		private void Awake()
		{
			entity = GetComponent<Entity>();
			inventory = GetComponent<Inventory>();
			mapManager = Locator.Get<MapManager>();
		}

		private void Start()
		{
			Initialize();
		}

		private void Initialize()
		{
			entity.Events.AddListener(OnMovementFinish, EntityFinishMovementEvent.EVENT_TYPE);
		}

		private void OnMovementFinish(IEventData data)
		{
			if (enabled)
			{
				EntityFinishMovementEvent movementEvent = data as EntityFinishMovementEvent;

				if (mapManager.Map[movementEvent.CurrentPosition].Items.Count > 0)
				{
					List<Entity> items = new List<Entity>(mapManager.Map[movementEvent.CurrentPosition].Items);

					for (int i = 0; i < items.Count; i++)
					{
						if (inventory.AvailableSlotsCount > 0)
						{
							if (mapManager.RemoveItem(items[i], movementEvent.CurrentPosition))
							{
								Item itemToRemove = items[i].GetComponent<Item>();

								if (itemToRemove != null && itemToRemove.Info != null)
								{
									inventory.Add(itemToRemove.Info);
									Locator.Get<EntityManager>().Recycle(items[i].Guid);

									// TODO : add item picked event
								}
							}
						}
						else
						{
							// TODO : add inventory full event
							break;
						}
					}
				}
			}
		}
	}
}
