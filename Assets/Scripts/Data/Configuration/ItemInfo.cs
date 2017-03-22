using UnityEngine;

namespace InstaDungeon.Configuration
{
	[CreateAssetMenu(menuName = "InstaDungeon/Configuration/ItemInfo", fileName = "new Item", order = 1000)]
	public class ItemInfo : ScriptableObject
	{
		public uint Id { get { return id; } }
		public string NameId { get { return nameId; } }

		[SerializeField] protected uint id;
		[SerializeField] protected string nameId;
	}
}
