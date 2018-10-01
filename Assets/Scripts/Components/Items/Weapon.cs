using InstaDungeon.Configuration;
using UnityEngine;

namespace InstaDungeon.Models
{
	[System.Serializable]
	public class Weapon : Item
	{
		public override ItemInfo ItemInfo { get { return weaponInfo; } }
		public WeaponInfo WeaponInfo { get { return weaponInfo; } }
		public int AdditionalRange { get { return additionalRange; } }
		public int AdditionalMinDamage { get { return additionalMinDamage; } }
		public int AdditionalMaxDamage { get { return additionalMaxDamage; } }

		public int Range { get { return weaponInfo.BaseRange + additionalRange; } }
		public int MinDamage { get { return weaponInfo.BaseMinDamage + additionalMinDamage; } }
		public int MaxDamage { get { return weaponInfo.BaseMaxDamage + additionalMaxDamage; } }

		[SerializeField] private WeaponInfo weaponInfo;
		[SerializeField] private int additionalRange;
		[SerializeField] private int additionalMinDamage;
		[SerializeField] private int additionalMaxDamage;

		public void Initialize(string itemName, WeaponInfo weaponInfo, int additionalRange, int additionalMinDamage, int additionalMaxDamage)
		{
			this.itemName = itemName;
			this.weaponInfo = weaponInfo;
			this.additionalMinDamage = additionalMinDamage;
			this.additionalMaxDamage = additionalMaxDamage;
		}

		public int GetRandomDamage()
		{
			return Random.Range(MinDamage, MaxDamage + 1);
		}
 	}
}
