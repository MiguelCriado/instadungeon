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
		Key,
		Gold,
		Head,
		Body,
		MainHand,
		OffHand
	}

	public class Inventory : MonoBehaviour, ISerializationCallbackReceiver
	{
		public static readonly InventorySlotType[] EquipSlotTypes = new InventorySlotType[]
		{
			InventorySlotType.Head,
			InventorySlotType.Body,
			InventorySlotType.MainHand,
			InventorySlotType.OffHand
		};

		public int BagCapacity { get { return bagCapacity; } }
		public int AvailableBagSlots { get { return bagCapacity - bag.Count; } }

		// Containers
		[SerializeField] private Transform inventoryContainer;
		[SerializeField] private Transform equipmentContainer;
		[SerializeField] private Transform bagContainer;
		[SerializeField] private Transform specialContainer;
		// Equipment
		[SerializeField] private List<InventorySlotType> keys = new List<InventorySlotType>();
		[SerializeField] private List<Item> values = new List<Item>();
		private Dictionary<InventorySlotType, Item> equipment = new Dictionary<InventorySlotType, Item>();
		private List<Item> equipmentCache = new List<Item>();
		private bool equipmentDirty = true;
		// Bag
		[SerializeField] private List<Item> bag;
		[SerializeField] private int bagCapacity;
		// Special Items
		private Dictionary<KeyInfo, Key> lockKeys = new Dictionary<KeyInfo, Key>();
		private List<Key> lockKeysCache = new List<Key>();
		private bool lockKeysDirty = true;
		//private Gold gold;
		// Managers
		private EntityManager entityManager;

		private void OnValidate()
		{
			RefreshEquipment();
		}

		private void Reset()
		{
			RetrieveContainers();
			RefreshEquipment();
		}

		private void Awake()
		{
			entityManager = Locator.Get<EntityManager>();
			RetrieveContainers();
			RefreshEquipment();
		}

		#region [Common Operations]

		public bool AddItem(Item item)
		{
			bool result = false;
			Type itemType = item.GetType();

			//if (itemType == typeof(Gold))
			//{
			//	// TODO
			//}
			//else
			if (itemType == typeof(Key))
			{
				AddKey(item as Key);
				result = true;
			}
			else if (item.ItemInfo.InventorySlot == InventorySlotType.Bag)
			{
				result = AddToBag(item);
			}
			else
			{
				if (Array.Exists(EquipSlotTypes, x => x == item.ItemInfo.InventorySlot) == true)
				{
					result = EquipItem(item, item.ItemInfo.InventorySlot);
				}
			}

			return result;
		}

		public bool Contains(ItemInfo itemInfo)
		{
			return EquipmentContains(itemInfo) || BagContains(itemInfo) || KeysContains(itemInfo);
		}

		#endregion

		#region [Equipment Operations]

		public Item GetEquippedItem(InventorySlotType slot)
		{
			Item result = null;

			if (Array.Exists(EquipSlotTypes, x => x == slot) == true)
			{
				result = equipment[slot];
			}
			else
			{
				Locator.Log.Error(string.Format("Equipped item slot cannot be of type {0}", slot));
			}

			return result;
		}

		public bool EquipmentContains(ItemInfo itemInfo)
		{
			return GetEquippedItems().Find(x => x.ItemInfo == itemInfo) != null;
		}

		public List<Item> GetEquippedItems()
		{
			if (equipmentDirty == true)
			{
				equipmentCache = new List<Item>(equipment.Values);
				equipmentDirty = false;
			}

			return equipmentCache;
		}

		public bool EquipItem(Item item, InventorySlotType slot)
		{
			bool result = false;

			if 
			(
				item.ItemInfo.InventorySlot == slot
				&& Array.Exists(EquipSlotTypes, x => x == item.ItemInfo.InventorySlot) == true
			)
			{
				Item itemInSlot = equipment[slot];

				if (itemInSlot != null)
				{
					throw new Exception(string.Format("Slot already occupied by {0}", itemInSlot));
				}

				equipment[slot] = item;
				AttachToInventory(item, equipmentContainer);
				equipmentDirty = true;

				result = true;
			}
			else
			{
				Debug.LogError(string.Format("slot parameter ({0}) doesn't match item's slot ({1})", slot, item.ItemInfo.InventorySlot));
			}

			return result;
		}

		public void UnequipItem(Item item)
		{
			// TODO get the item to the bag
		}

		#endregion

		#region [Bag Operations]

		public List<Item> GetBagItems()
		{
			return bag;
		}

		public bool AddToBag(Item item)
		{
			bool result = false;

			if (bag.Count < bagCapacity)
			{
				bag.Add(item);
				AttachToInventory(item, bagContainer);
				result = true;
			}

			return result;
		}

		public bool RemoveFromBag(Item item)
		{
			bool result = bag.Remove(item);
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

		public List<Item> FindInBag(string nameId)
		{
			return bag.FindAll(x => x.ItemInfo.NameId == nameId);
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

		#region [Special Items Operations]

		// TODO: gold operations

		public Key GetKey(KeyInfo keyType)
		{
			Key result = null;
			lockKeys.TryGetValue(keyType, out result);
			return result;
		}

		public void AddKey(Key key)
		{
			if (lockKeys.ContainsKey(key.KeyInfo))
			{
				lockKeys[key.KeyInfo].Amount += key.Amount;
				entityManager.Recycle(key.GetComponent<Entity>().Guid);
			}
			else
			{
				lockKeys[key.KeyInfo] = key;
				AttachToInventory(key, specialContainer);
				lockKeysDirty = true;
			}
		}

		public Key RemoveKey(KeyInfo keyType, int amount)
		{
			Key result = null;

			if (lockKeys.TryGetValue(keyType, out result))
			{
				int finalAmount = result.Amount - amount;
				
				if (finalAmount == 0)
				{
					lockKeys.Remove(keyType);
					result.transform.SetParent(null);
					result.gameObject.SetActive(true);
				}
				else
				{
					result.Amount = finalAmount;
					result = entityManager.Spawn(keyType.NameId).GetComponent<Key>();
					result.Amount = amount;
				}
			}

			return result;
		}

		public Key FindKey(string nameId)
		{
			return GetAllKeys().Find(x => x.ItemInfo.NameId == nameId);
		}

		public bool KeysContains(ItemInfo itemInfo)
		{
			return GetAllKeys().Find(x => x.ItemInfo == itemInfo) != null;
		}

		public List<Key> GetAllKeys()
		{
			if (lockKeysDirty == true)
			{
				lockKeysCache = new List<Key>(lockKeys.Values);
				lockKeysDirty = false;
			}

			return lockKeysCache;
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

		private void RetrieveContainers()
		{
			inventoryContainer = transform.GetOrCreateContainer("Inventory");
			equipmentContainer = inventoryContainer.GetOrCreateContainer("Equipment");
			bagContainer = inventoryContainer.GetOrCreateContainer("Bag");
			specialContainer = inventoryContainer.GetOrCreateContainer("Special Items");
		}

		private void AttachToInventory(Item item, Transform container)
		{
			item.transform.SetParent(container);
			item.transform.localPosition = Vector3.zero;
			item.gameObject.SetActive(false);
		}

		private void RefreshEquipment()
		{
			OnAfterDeserialize();

			if (equipment.Count != EquipSlotTypes.Length - 1)
			{
				for (int i = 0; i < EquipSlotTypes.Length; i++)
				{
					if (!equipment.ContainsKey(EquipSlotTypes[i]))
					{
						equipment[EquipSlotTypes[i]] = null;
					}
				}

				equipmentDirty = true;
			}

			OnBeforeSerialize();
		}

		#endregion
	}
}
