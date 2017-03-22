using InstaDungeon.Configuration;
using UnityEngine;

namespace InstaDungeon.Components
{
	public class Item : MonoBehaviour
	{
		public ItemInfo Info { get { return info; } }

		[SerializeField] private ItemInfo info;
	}
}
