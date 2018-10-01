using InstaDungeon.Configuration;
using InstaDungeon.Events;
using InstaDungeon.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace InstaDungeon.Components
{
	public enum InventorySlotType
	{
		None,
		Bag,
		Key,
		Gold,
		Head,
		Body,
		MainHand,
		OffHand
	}

	[RequireComponent(typeof(Entity))]
	public class Inventory : MonoBehaviour, ISerializationCallbackReceiver
	{
		[SerializeField] private List<InventorySlotType> keys = new List<InventorySlotType>();
		[SerializeField] private List<Item> values = new List<Item>();

		private Entity entity;
		private Transform inventoryContainer;
		private Dictionary<InventorySlotType, Item> items = new Dictionary<InventorySlotType, Item>();
		private List<Item> itemsCache = new List<Item>();
		private bool itemsDirty = true;

		private void OnValidate()
		{
			RefreshItemsSlots();
		}

		private void Reset()
		{
			RetrieveContainer();
			RefreshItemsSlots();
		}

		private void Awake()
		{
			entity = GetComponent<Entity>();
			RetrieveContainer();
			RefreshItemsSlots();
		}

		#region [Public API]

		public Item GetItem(InventorySlotType slot)
		{
			Item result;
			items.TryGetValue(slot, out result);
			return result;
		}

		public Item FindItem(string itemNameId)
		{
			return GetItemsCache().Find(x => x.ItemInfo.NameId == itemNameId);
		}

		public bool AddItem(Item item)
		{
			bool result = false;
			InventorySlotType itemSlot = item.ItemInfo.InventorySlot;

			if (items.ContainsKey(itemSlot) && items[itemSlot] != null)
			{
				throw new ArgumentException(string.Format("Slot already occupied by {0}", items[itemSlot]));
			}
			else
			{
				items[itemSlot] = item;
				AttachToInventory(item, inventoryContainer);
				itemsDirty = true;
				result = true;
				entity.Events.TriggerEvent(new InventoryItemAddEvent(entity, this, itemSlot, item));
			}

			return result;
		}

		public bool AddItemAmount(Item item)
		{
			bool result = false;
			InventorySlotType slot = item.ItemInfo.InventorySlot;

			if (items[slot] != null 
				&& items[slot ].ItemInfo.Id == item.ItemInfo.Id 
				&& items[slot].ItemInfo.Stackable == true)
			{
				items[slot].Amount += item.Amount;
				Locator.Get<EntityManager>().Recycle(item.GetComponent<Entity>().Guid);
				result = true;
				// TODO: trigger Event
			}

			return result;
		}

		public bool RemoveItem(Item item)
		{
			bool result = false;
			InventorySlotType itemSlot = item.ItemInfo.InventorySlot;

			if (items.ContainsKey(itemSlot) && items[itemSlot] != null)
			{
				Item itemToRemove = items[itemSlot];
				itemToRemove.transform.SetParent(null);
				itemToRemove.gameObject.SetActive(true);
				items[itemSlot] = null;
				itemsDirty = true;
				result = true;
				entity.Events.TriggerEvent(new InventoryItemRemoveEvent(entity, this, itemSlot, item));
			}

			return result;
		}

		public Item RemoveItem(ItemInfo itemInfo)
		{
			Item result = GetItemsCache().Find(x => x.ItemInfo == itemInfo);

			if (result != null)
			{
				RemoveItem(result);
			}

			return result;
		}

		public bool Contains(ItemInfo itemInfo)
		{
			return GetItemsCache().Find(x => x.ItemInfo == itemInfo);
		}

		public bool Contains(Item item)
		{
			return GetItemsCache().Contains(item);
		}

		public List<Item> GetItemsCache()
		{
			if (itemsDirty == true)
			{
				itemsCache = new List<Item>();
				var enumerator = items.Values.GetEnumerator();

				while (enumerator.MoveNext())
				{
					if (enumerator.Current != null)
					{
						itemsCache.Add(enumerator.Current);
					}
				}

				itemsDirty = false;
			}

			return itemsCache;
		}

		public void Clear()
		{
			List<Item> items = GetItemsCache();

			for (int i = 0; i < items.Count; i++)
			{
				RemoveItem(items[i]);
				Locator.Get<EntityManager>().Recycle(items[i].GetComponent<Entity>().Guid);
			}
		}

		#endregion

		#region [Serialization]

		public void OnBeforeSerialize()
		{
			keys.Clear();
			values.Clear();

			var enumerator = items.GetEnumerator();

			while (enumerator.MoveNext())
			{
				keys.Add(enumerator.Current.Key);
				values.Add(enumerator.Current.Value);
			}
		}

		public void OnAfterDeserialize()
		{
			items = new Dictionary<InventorySlotType, Item>();

			for (int i = 0; i < Math.Min(keys.Count, values.Count); i++)
			{
				items.Add(keys[i], values[i]);
			}
		}

		#endregion

		#region [Helpers]

		private void RetrieveContainer()
		{
			inventoryContainer = transform.GetOrCreateContainer("Inventory");
		}

		private void AttachToInventory(Item item, Transform container)
		{
			item.transform.SetParent(container);
			item.transform.localPosition = Vector3.zero;
			item.gameObject.SetActive(false);
		}

		private void RefreshItemsSlots()
		{
			OnAfterDeserialize();

			Array slotTypes = Enum.GetValues(typeof(InventorySlotType));

			if (items.Count != slotTypes.Length)
			{
				var enumerator = slotTypes.GetEnumerator();
				
				while (enumerator.MoveNext())
				{
					items[(InventorySlotType)enumerator.Current] = null;
				}

				itemsDirty = true;
			}

			OnBeforeSerialize();
		}

		#endregion
	}
}
