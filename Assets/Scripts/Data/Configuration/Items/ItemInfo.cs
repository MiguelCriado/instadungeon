using InstaDungeon.Components;
using System;
using UnityEngine;

namespace InstaDungeon.Configuration
{
	[CreateAssetMenu(menuName = "InstaDungeon/Configuration/Items/ItemInfo", fileName = "new Item", order = 1000)]
	public class ItemInfo : ScriptableObject
	{
		public uint Id { get { return id; } }
		public string NameId { get { return nameId; } }
		public Sprite Avatar { get { return avatar; } }
		public InventorySlotType InventorySlot { get { return inventorySlot; } }
		public bool Stackable { get { return stackable; } }

		[SerializeField] protected uint id;
		[SerializeField] protected string nameId;
		[SerializeField] protected Sprite avatar;
		[SerializeField] protected InventorySlotType inventorySlot;
		[SerializeField] protected bool stackable;

		protected virtual void Reset()
		{
			id = (uint)((Guid.NewGuid().GetHashCode() ^ DateTime.UtcNow.Millisecond) + int.MaxValue + 1);
			nameId = Guid.NewGuid().ToString();
			avatar = null;
			inventorySlot = InventorySlotType.None;
			stackable = false;
		}
	}
}
