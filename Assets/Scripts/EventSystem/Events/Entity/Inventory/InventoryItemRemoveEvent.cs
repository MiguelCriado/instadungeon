using InstaDungeon.Components;
using InstaDungeon.Models;

namespace InstaDungeon.Events
{
	public class InventoryItemRemoveEvent : BaseEventData
	{
		public static readonly uint EVENT_TYPE = 0XD62F690B;

		public override uint EventType { get { return EVENT_TYPE; } }
		public override string Name { get { return "Inventory Item Remove Event"; } }

		public Entity Entity { get; private set; }
		public Inventory Inventory { get; private set; }
		public InventorySlotType Slot { get; private set; }
		public Item Item { get; private set; }

		public InventoryItemRemoveEvent(Entity entity, Inventory inventory, InventorySlotType slot, Item item)
		{
			Entity = entity;
			Inventory = inventory;
			Slot = slot;
			Item = item;
		}

		public override BaseEventData CopySpecificData()
		{
			return new InventoryItemRemoveEvent(Entity, Inventory, Slot, Item);
		}
	}
}
