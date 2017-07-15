using InstaDungeon.Configuration;
using UnityEngine;

namespace InstaDungeon.Models
{
	public abstract class Item : MonoBehaviour
	{
		public string ItemName { get { return itemName; } }
		public abstract ItemInfo ItemInfo { get; }

		[SerializeField] protected string itemName;
	}
}
