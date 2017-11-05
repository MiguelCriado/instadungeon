using AI.BehaviorTrees;
using InstaDungeon.Components;
using InstaDungeon.Models;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class ConsumeItemInBagAction : ActionNode
	{
		protected override NodeStates Tick(Tick tick)
		{
			NodeStates result = NodeStates.Failure;
			Entity target = tick.Target as Entity;
			Actor actor = target.GetComponent<Actor>();
			Inventory inventory = target.GetComponent<Inventory>();

			if (actor != null && inventory != null)
			{
				Item item = inventory.GetItem(InventorySlotType.Bag);

				if (item != null && item is IConsumable)
				{
					actor.ConsumeItemInBag();
					result = NodeStates.Success;
				}
				else
				{
					result = NodeStates.Error;
				}
			}
			else
			{
				result = NodeStates.Error;
			}

			return result;
		}
	}
}
