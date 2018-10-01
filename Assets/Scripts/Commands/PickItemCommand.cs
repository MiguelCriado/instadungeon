using InstaDungeon.Components;
using InstaDungeon.Models;

namespace InstaDungeon.Commands
{
	public class PickItemCommand : Command
	{
		public Entity Actor { get; private set; }
		public Entity Item { get; private set; }

		public PickItemCommand(Entity actor, Entity item)
		{
			Actor = actor;
			Item = item;
		}

		public override void Execute()
		{
			base.Execute();

			MapManager mapManager = Locator.Get<MapManager>();
			Inventory inventory = Actor.GetComponent<Inventory>();
			Item itemComponent = Item.GetComponent<Item>();

			if (inventory != null && itemComponent != null)
			{
				InventorySlotType slot = itemComponent.ItemInfo.InventorySlot;
				Item presentItem = inventory.GetItem(slot);

				if (presentItem != null)
				{
					inventory.RemoveItem(presentItem);
					Entity itemEntity = presentItem.GetComponent<Entity>();
					mapManager.AddItem(itemEntity, Item.CellTransform.Position);
				}

				mapManager.RemoveItem(Item, Item.CellTransform.Position);
				inventory.AddItem(itemComponent);
			}
		}

		public override void Undo()
		{
			base.Undo();

			// TODO
		}
	}
}
