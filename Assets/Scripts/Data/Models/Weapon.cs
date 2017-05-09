using InstaDungeon.Configuration;
using UnityEngine;

namespace InstaDungeon.Models
{
	[System.Serializable]
	public class Weapon
	{
		public WeaponInfo Info { get { return info; } }
		public int AdditionalRange { get { return additionalRange; } }
		public int AdditionalMinDamage { get { return additionalMinDamage; } }
		public int AdditionalMaxDamage { get { return additionalMaxDamage; } }

		public int Range { get { return info.BaseRange + additionalRange; } }
		public int MinDamage { get { return info.BaseMinDamage + additionalMinDamage; } }
		public int MaxDamage { get { return info.BaseMaxDamage + additionalMaxDamage; } }

		[SerializeField] private WeaponInfo info;
		[SerializeField] private int additionalRange;
		[SerializeField] private int additionalMinDamage;
		[SerializeField] private int additionalMaxDamage;

		public int GetRandomDamage()
		{
			return Random.Range(MinDamage, MaxDamage + 1);
		}
 	}
}
