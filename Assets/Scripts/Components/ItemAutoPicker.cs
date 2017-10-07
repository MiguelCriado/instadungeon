using InstaDungeon.Events;
using InstaDungeon.Models;
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
		private ItemInteractor itemInteraction;

		private void Awake()
		{
			entity = GetComponent<Entity>();
			inventory = GetComponent<Inventory>();
			mapManager = Locator.Get<MapManager>();
			itemInteraction = GetComponent<ItemInteractor>();
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
						Item item = items[i].GetComponent<Item>();

						if (item != null)
						{
							if (inventory.GetItem(item.ItemInfo.InventorySlot) == null)
							{
								if (mapManager.RemoveItem(items[i], movementEvent.CurrentPosition))
								{
									inventory.AddItem(item);
									AddItemAnimation(item);

									// TODO : add item picked event
								}
							}
						}
					}
				}
			}
		}

		private void AddItemAnimation(Item item)
		{
			if (itemInteraction != null)
			{
				itemInteraction.AddItem(item.ItemInfo);
			}
		}
	}
}
