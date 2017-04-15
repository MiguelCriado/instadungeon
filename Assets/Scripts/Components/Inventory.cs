using InstaDungeon.Configuration;
using System.Collections.Generic;
using UnityEngine;

namespace InstaDungeon.Components
{
	public class Inventory : MonoBehaviour
	{
		public int Capacity { get { return capacity; } }
		public int AvailableSlotsCount { get { return capacity - items.Count; } }

		[SerializeField] private int capacity;
		[SerializeField] private List<ItemInfo> items;

		public bool Add(ItemInfo item)
		{
			bool result = false;

			if (items.Count < capacity)
			{
				items.Add(item);
				result = true;
			}

			return result;
		}

		public bool Contains(ItemInfo item)
		{
			return items.Contains(item);
		}

		public bool Remove(ItemInfo item)
		{
			return items.Remove(item);
		}
	}
}
