using InstaDungeon.Components;
using InstaDungeon.Models;

namespace InstaDungeon.Events
{
	public class InventoryItemAddEvent : BaseEventData
	{
		public static readonly uint EVENT_TYPE = 0XF184B9E6;

		public override uint EventType { get { return EVENT_TYPE; } }
		public override string Name { get { return "Inventory Item Add Event"; } }

		public Entity Entity { get; private set; }
		public Inventory Inventory { get; private set; }
		public InventorySlotType Slot { get; private set; }
		public Item Item { get; private set; }

		public InventoryItemAddEvent(Entity entity, Inventory inventory, InventorySlotType slot, Item item)
		{
			Entity = entity;
			Inventory = inventory;
			Slot = slot;
			Item = item;
		}

		public override BaseEventData CopySpecificData()
		{
			return new InventoryItemAddEvent(Entity, Inventory, Slot, Item);
		}
	}
}
