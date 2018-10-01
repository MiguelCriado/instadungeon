using AI.BehaviorTrees;
using InstaDungeon.Components;
using InstaDungeon.Models;
using System;
using System.Collections.Generic;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class InventoryContainsCondition : ConditionNode
	{
		private Predicate<Item> itemMatch;
		private List<InventorySlotType> slotsToLookUp;

		/// <summary>
		/// Checks a predicate for inventory items
		/// </summary>
		/// <param name="itemNameId">The name Id of the item to look up. As ItemInfo.NameId</param>
		/// <param name="slotsToLookUp"></param>
		public InventoryContainsCondition(Predicate<Item> itemMatch, params InventorySlotType[] slotsToLookUp)
		{
			this.itemMatch = itemMatch;
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

					if (item != null && itemMatch(item))
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
