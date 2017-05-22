using UnityEngine;

namespace InstaDungeon.Configuration
{
	public enum WeaponType
	{
		None,
		Blade,
		Blunt,
		Range
	}

	[CreateAssetMenu(menuName = "InstaDungeon/Configuration/Items/WeaponInfo", fileName = "new Weapon", order = 1000)]
	public class WeaponInfo : ItemInfo
	{
		public WeaponType WeaponType { get { return weaponType; } }
		public int BaseRange { get { return baseRange; } }
		public int BaseMinDamage { get { return baseMinDamage; } }
		public int BaseMaxDamage { get { return baseMaxDamage; } }

		[SerializeField] private WeaponType weaponType;
		[SerializeField] private int baseRange;
		[SerializeField] private int baseMinDamage;
		[SerializeField] private int baseMaxDamage;

		protected override void Reset()
		{
			base.Reset();
			weaponType = WeaponType.None;
			baseRange = 1;
			baseMinDamage = 1;
			baseMaxDamage = 1;
		}
	}
}
