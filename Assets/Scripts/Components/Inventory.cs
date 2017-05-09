using InstaDungeon.Configuration;
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

	public class Inventory : MonoBehaviour
	{
		public int BagCapacity { get { return bagCapacity; } }
		public int AvailableSlotsCount { get { return bagCapacity - items.Count; } }

		[SerializeField] private int bagCapacity;
		[SerializeField] private Dictionary<InventorySlotType, List<ItemInfo>> items;

		private List<ItemInfo> bagCache;

		private void Awake()
		{
			items = new Dictionary<InventorySlotType, List<ItemInfo>>();

			foreach (var value in Enum.GetValues(typeof(InventorySlotType)))
			{
				items[(InventorySlotType)value] = new List<ItemInfo>();
			}

			bagCache = items[InventorySlotType.Bag];
		}

		public bool AddToBag(ItemInfo item)
		{
			bool result = false;

			if (bagCache.Count < bagCapacity)
			{
				bagCache.Add(item);
				result = true;
			}

			return result;
		}

		public bool BagContains(ItemInfo item)
		{
			return bagCache.Contains(item);
		}

		public bool RemoveFromBag(ItemInfo item)
		{
			return bagCache.Remove(item);
		}
	}
}
