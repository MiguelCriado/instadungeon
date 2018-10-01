using InstaDungeon.Components;
using InstaDungeon.Events;
using InstaDungeon.Models;
using UnityEngine;
using UnityEngine.UI;

namespace InstaDungeon.UI
{
	public class InventorySlotController : MonoBehaviour
	{
		[SerializeField] private Image itemAvatar;
		[SerializeField] private InventorySlotType monitoredSlot;

		private Entity entity;

		private void OnDestroy()
		{
			if (entity != null)
			{
				UnsubscribeListeners(entity);
			}

			entity = null;
		}

		public void Initialize(Entity entity)
		{
			this.entity = entity;
			Refresh();
			SubscribeListeners(entity);
		}

		private void Refresh()
		{
			if (entity != null)
			{
				Inventory inventory = entity.GetComponent<Inventory>();
				
				if (inventory != null)
				{
					Item item = inventory.GetItem(monitoredSlot);

					if (item != null)
					{
						itemAvatar.gameObject.SetActive(true);
						itemAvatar.overrideSprite = item.ItemInfo.Avatar;
					}
					else
					{
						itemAvatar.gameObject.SetActive(false);
						itemAvatar.overrideSprite = null;
					}
				}
			}
		}

		private void OnItemAddEvent(IEventData eventData)
		{
			InventoryItemAddEvent itemEvent = eventData as InventoryItemAddEvent;

			if (itemEvent.Slot == monitoredSlot)
			{
				itemAvatar.gameObject.SetActive(true);
				itemAvatar.overrideSprite = itemEvent.Item.ItemInfo.Avatar;
			}
		}

		private void OnItemRemoveEvent(IEventData eventData)
		{
			InventoryItemRemoveEvent itemEvent = eventData as InventoryItemRemoveEvent;

			if (itemEvent.Slot == monitoredSlot)
			{
				itemAvatar.gameObject.SetActive(false);
				itemAvatar.overrideSprite = null;
			}
		}

		private void SubscribeListeners(Entity entity)
		{
			entity.Events.AddListener(OnItemAddEvent, InventoryItemAddEvent.EVENT_TYPE);
			entity.Events.AddListener(OnItemRemoveEvent, InventoryItemRemoveEvent.EVENT_TYPE);
		}

		private void UnsubscribeListeners(Entity entity)
		{
			entity.Events.RemoveListener(OnItemAddEvent, InventoryItemAddEvent.EVENT_TYPE);
			entity.Events.RemoveListener(OnItemRemoveEvent, InventoryItemRemoveEvent.EVENT_TYPE);
		}
	}
}
