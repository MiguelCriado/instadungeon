using InstaDungeon.Configuration;
using UnityEngine;

namespace InstaDungeon.Models
{
	public class Gold : Item
	{
		public override ItemInfo ItemInfo { get { return itemInfo; } }

		[SerializeField] private ItemInfo itemInfo;
	}
}
