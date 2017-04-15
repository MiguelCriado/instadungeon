using System;
using UnityEngine;

namespace InstaDungeon.Configuration
{
	[CreateAssetMenu(menuName = "InstaDungeon/Configuration/ItemInfo", fileName = "new Item", order = 1000)]
	public class ItemInfo : ScriptableObject
	{
		public uint Id { get { return id; } }
		public string NameId { get { return nameId; } }
		public Sprite Avatar { get { return avatar; } }

		[SerializeField] protected uint id;
		[SerializeField] protected string nameId;
		[SerializeField] protected Sprite avatar;

		private void Reset()
		{
			id = (uint)((Guid.NewGuid().GetHashCode() ^ DateTime.UtcNow.Millisecond) + int.MaxValue + 1);
			nameId = Guid.NewGuid().ToString();
			avatar = null;
		}
	}
}
