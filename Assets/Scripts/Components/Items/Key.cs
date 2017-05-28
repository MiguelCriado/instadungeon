using InstaDungeon.Configuration;
using UnityEngine;

namespace InstaDungeon.Models
{
	public class Key : Item
	{
		public override ItemInfo ItemInfo { get { return keyInfo; } }
		public ItemInfo KeyInfo { get { return keyInfo; } }

		[SerializeField] private ItemInfo keyInfo;

		public void Initialize(string itemName, ItemInfo keyInfo)
		{
			this.itemName = itemName;
			this.keyInfo = keyInfo;
		}
	}
}
