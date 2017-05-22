using InstaDungeon.Configuration;
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
		Head,
		Body,
		MainHand,
		OffHand
	}

	public class Inventory : MonoBehaviour, ISerializationCallbackReceiver
	{
		public int BagCapacity { get { return bagCapacity; } }
		public int AvailableBagSlots { get { return bagCapacity - bag.Count; } }

		[SerializeField] private List<Item> bag;
		[SerializeField] private int bagCapacity;

		[SerializeField] private List<InventorySlotType> keys = new List<InventorySlotType>();
		[SerializeField] private List<Item> values = new List<Item>();

		[SerializeField] private Transform inventoryContainer;
		[SerializeField] private Transform equipmentContainer;
		[SerializeField] private Transform bagContainer;

		private Dictionary<InventorySlotType, Item> equipment = new Dictionary<InventorySlotType, Item>();

		private void OnValidate()
		{
			RefreshEquipment();
		}

		private void Reset()
		{
			inventoryContainer = transform.GetOrCreateContainer("Inventory");
			equipmentContainer = inventoryContainer.GetOrCreateContainer("Equipment");
			bagContainer = inventoryContainer.GetOrCreateContainer("Bag");
			RefreshEquipment();
		}

		private void Awake()
		{
			RefreshEquipment();
		}

		#region [Equipment Operations]

		public bool EquipItem(Item item, InventorySlotType slot)
		{
			bool result = false;

			if (item.ItemInfo.InventorySlot == slot)
			{
				Item itemInSlot = equipment[slot];

				if (itemInSlot != null)
				{
					if (AvailableBagSlots > 0)
					{
						AddToBag(itemInSlot);
					}
					else
					{
						Debug.Log(string.Format("Bag full. Item {0} cannot be unequipped", itemInSlot));
						return false; // TODO throw exception
					}
				}

				equipment[slot] = item;
				item.transform.SetParent(equipmentContainer);
				item.transform.localPosition = Vector3.zero;
				item.gameObject.SetActive(false);

				result = true;
			}
			else
			{
				Debug.Log(string.Format("slot parameter ({0}) doesn't match item's slot ({1})", slot, item.ItemInfo.InventorySlot));
			}

			return result;
		}

		public void UnequipItem(Item item)
		{
			// TODO get the item to the bag
		}

		#endregion

		#region [Bag Operations]

		public bool AddToBag(Item item)
		{
			bool result = false;

			if (bag.Count < bagCapacity)
			{
				bag.Add(item);
				item.transform.SetParent(bagContainer);
				item.transform.localPosition = Vector3.zero;
				item.gameObject.SetActive(false);
				result = true;
			}

			return result;
		}

		public bool RemoveFromBag(Item item)
		{
			bool result =  bag.Remove(item);
			item.transform.SetParent(null);
			item.gameObject.SetActive(true);
			return result;
		}

		public bool BagContains(Item item)
		{
			return bag.Contains(item);
		}

		public bool BagContains(ItemInfo item)
		{
			return bag.Find(x => x.ItemInfo == item) != null;
		}

		public Item RemoveFromBag(ItemInfo item)
		{
			Item result = bag.Find(x => x.ItemInfo == item);

			if (result != null)
			{
				RemoveFromBag(result);
			}

			return result;
		}

		#endregion

		#region [Serialization]

		public void OnBeforeSerialize()
		{
			keys.Clear();
			values.Clear();

			var enumerator = equipment.GetEnumerator();

			while (enumerator.MoveNext())
			{
				keys.Add(enumerator.Current.Key);
				values.Add(enumerator.Current.Value);
			}
		}

		public void OnAfterDeserialize()
		{
			equipment = new Dictionary<InventorySlotType, Item>();

			for (int i = 0; i < Math.Min(keys.Count, values.Count); i++)
			{
				equipment.Add(keys[i], values[i]);
			}
		}

		#endregion

		#region [Helpers]

		private void RefreshEquipment()
		{
			OnAfterDeserialize();

			Array inventoryValues = Enum.GetValues(typeof(InventorySlotType));

			if (equipment.Count != inventoryValues.Length - 1)
			{
				foreach (var value in inventoryValues)
				{
					InventorySlotType enumValue = (InventorySlotType)value;

					if (enumValue != InventorySlotType.None && enumValue != InventorySlotType.Bag && !equipment.ContainsKey(enumValue))
					{
						equipment[enumValue] = null;
					}
				}
			}

			OnBeforeSerialize();
		}

		#endregion
	}
}
