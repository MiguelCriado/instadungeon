using System;
using UnityEngine;

namespace InstaDungeon
{
	public enum WeaponType
	{
		None,
		Blade,
		Blunt,
		Range
	}

	[CreateAssetMenu(menuName = "InstaDungeon/Configuration/WeaponInfo", fileName = "new Weapon", order = 1000)]
	public class WeaponInfo : ScriptableObject
	{
		public uint Id { get { return id; } }
		public string NameId { get { return nameId; } }
		public Sprite Avatar { get { return avatar; } }
		public WeaponType WeaponType { get { return weaponType; } }
		public int BaseRange { get { return baseRange; } }
		public int BaseMinDamage { get { return baseMinDamage; } }
		public int BaseMaxDamage { get { return baseMaxDamage; } }

		[SerializeField] private uint id;
		[SerializeField] private string nameId;
		[SerializeField] private Sprite avatar;
		[SerializeField] private WeaponType weaponType;
		[SerializeField] private int baseRange;
		[SerializeField] private int baseMinDamage;
		[SerializeField] private int baseMaxDamage;

		private void Reset()
		{
			id = (uint)((Guid.NewGuid().GetHashCode() ^ DateTime.UtcNow.Millisecond) + int.MaxValue + 1);
			nameId = Guid.NewGuid().ToString();
			avatar = null;
			weaponType = WeaponType.None;
			baseRange = 1;
			baseMinDamage = 1;
			baseMaxDamage = 1;
		}
	}
}
