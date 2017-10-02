using InstaDungeon.Components;
using InstaDungeon.Configuration;
using InstaDungeon.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InstaDungeon.Commands
{
	public class PickItemCommand : Command
	{
		public Entity Actor { get; private set; }
		public Item Item { get; private set; }

		public PickItemCommand(Entity actor, Item item)
		{
			Actor = actor;
			Item = item;
		}

		public override void Execute()
		{
			base.Execute();

			Inventory inventory = Actor.GetComponent<Inventory>();

			if (inventory != null)
			{
				inventory.AddItem(Item);
			}
		}

		public override void Undo()
		{
			base.Undo();

			// TODO
		}
	}
}
