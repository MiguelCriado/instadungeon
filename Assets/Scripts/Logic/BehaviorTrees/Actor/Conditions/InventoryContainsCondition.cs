using AI.BehaviorTrees;
using InstaDungeon.Components;
using InstaDungeon.Models;
using System.Collections.Generic;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class InventoryContainsCondition : ConditionNode
	{
		private string itemNameId;
		private List<InventorySlotType> slotsToLookUp;

		/// <summary>
		/// Checks if an item is present in the actor's inventory
		/// </summary>
		/// <param name="itemNameId">The name Id of the item to look up. As ItemInfo.NameId</param>
		/// <param name="slotsToLookUp"></param>
		public InventoryContainsCondition(string itemNameId, params InventorySlotType[] slotsToLookUp)
		{
			this.itemNameId = itemNameId;
			this.slotsToLookUp = new List<InventorySlotType>(slotsToLookUp);
		}

		protected override NodeStates Tick(Tick tick)
		{
			NodeStates result = NodeStates.Failure;
			Entity target = tick.Target as Entity;
			Inventory inventory = target.GetComponent<Inventory>();

			if (inventory != null)
			{
				int i = 0;

				while (result == NodeStates.Failure && i < slotsToLookUp.Count)
				{
					Item item = inventory.GetItem(slotsToLookUp[i]);

					if (item != null && item.ItemInfo.NameId == itemNameId)
					{
						result = NodeStates.Success;
					}

					i++;
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
