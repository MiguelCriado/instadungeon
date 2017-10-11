using UnityEngine;

namespace InstaDungeon.Configuration
{
	[CreateAssetMenu(menuName = "InstaDungeon/Configuration/Items/HealthPotionInfo", fileName = "new HealthPotion", order = 1000)]
	public class HealthPotionInfo : ItemInfo
	{
		public int HealthRestored { get { return healthRestored; } }

		[SerializeField] int healthRestored;
	}
}
