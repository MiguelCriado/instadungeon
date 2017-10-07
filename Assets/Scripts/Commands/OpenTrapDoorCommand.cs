using InstaDungeon.Components;
using InstaDungeon.Configuration;
using InstaDungeon.Events;
using InstaDungeon.Models;
using UnityEngine;

namespace InstaDungeon.Commands
{
	public class OpenTrapDoorCommand : Command
	{
		public Entity Actor { get; private set; }
		public Entity TrapDoor { get; private set; }
		public ItemInfo RequiredItem { get; private set; }

		private Item usedItem;

		public OpenTrapDoorCommand(Entity actor, Entity trapDoor, ItemInfo requiredKey)
		{
			Actor = actor;
			TrapDoor = trapDoor;
			RequiredItem = requiredKey;
		}

		public override void Execute()
		{
			base.Execute();

			Inventory inventory = Actor.GetComponent<Inventory>();

			if (inventory != null)
			{
				if (inventory.Contains(RequiredItem))
				{
					usedItem = inventory.RemoveItem(RequiredItem);
					Locator.Get<EntityManager>().Recycle(usedItem.GetComponent<Entity>().Guid);

					TrapDoor.BlocksLineOfSight = false;
					TrapDoor.BlocksMovement = false;

					Animator animator = TrapDoor.GetComponent<Animator>();

					if (animator != null)
					{
						animator.SetBool("IsOpen", true);
					}

					TrapDoor trapDoor = TrapDoor.GetComponent<TrapDoor>();
					trapDoor.IsOpen = true;

					TrapDoor.Events.TriggerEvent(new TrapDoorOpenEvent(TrapDoor));
				}
			}
		}

		public override void Undo()
		{
			base.Undo();

			Inventory inventory = Actor.GetComponent<Inventory>();

			if (inventory != null)
			{
				Animator animator = TrapDoor.GetComponent<Animator>();

				if (animator != null)
				{
					animator.SetBool("IsOpen", false);
				}

				inventory.AddItem(usedItem);
			}
		}
	}
}
