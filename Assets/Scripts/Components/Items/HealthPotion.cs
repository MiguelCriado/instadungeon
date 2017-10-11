using InstaDungeon.Configuration;
using UnityEngine;

namespace InstaDungeon.Models
{
	public class HealthPotion : Item
	{
		public override ItemInfo ItemInfo { get { return healthPotionInfo; } }
		public HealthPotionInfo HealthPotionInfo { get { return healthPotionInfo; } }

		[SerializeField] private HealthPotionInfo healthPotionInfo;
	}
}
