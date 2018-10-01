using UnityEngine;
using System;

namespace InstaDungeon.Configuration
{
	[CreateAssetMenu(menuName = "InstaDungeon/Configuration/EntityInfo", fileName = "new Entity", order = 1000)]
	public class EntityInfo : ScriptableObject
	{
		public uint Id { get { return id; } }
		public string NameId { get { return nameId; } }

		[SerializeField] protected uint id;
		[SerializeField] protected string nameId;

		private void Reset()
		{
			id = (uint)((Guid.NewGuid().GetHashCode() ^ DateTime.UtcNow.Millisecond) + int.MaxValue + 1);
			nameId = Guid.NewGuid().ToString();
		}
	}
}
