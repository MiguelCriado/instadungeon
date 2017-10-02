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
		public KeyInfo RequiredKey { get; private set; }

		private Key usedKey;

		public OpenTrapDoorCommand(Entity actor, Entity trapDoor, KeyInfo requiredKey)
		{
			Actor = actor;
			TrapDoor = trapDoor;
			RequiredKey = requiredKey;
		}

		public override void Execute()
		{
			base.Execute();

			Inventory inventory = Actor.GetComponent<Inventory>();

			if (inventory != null)
			{
				if (inventory.KeysContains(RequiredKey))
				{
					usedKey = inventory.RemoveKey(RequiredKey, 1);
					Locator.Get<EntityManager>().Recycle(usedKey.GetComponent<Entity>().Guid);

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

				inventory.AddKey(usedKey);
			}
		}
	}
}
