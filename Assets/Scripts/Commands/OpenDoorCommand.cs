using InstaDungeon.Components;
using InstaDungeon.Configuration;
using UnityEngine;

namespace InstaDungeon.Commands
{
	public class OpenDoorCommand : Command
	{
		public Entity Actor { get; private set; }
		public Entity Door { get; private set; }
		public ItemInfo RequiredKey { get; private set; }

		private bool lastBlocksLineOfSight;
		private bool lastBlocksMovement;

		public OpenDoorCommand(Entity actor, Entity door, ItemInfo requiredKey)
		{
			Actor = actor;
			Door = door;
			RequiredKey = requiredKey;
		}

		public override void Execute()
		{
			base.Execute();

			Inventory inventory = Actor.GetComponent<Inventory>();

			if (inventory != null)
			{
				if (inventory.Contains(RequiredKey))
				{
					inventory.Remove(RequiredKey);

					Door.BlocksLineOfSight = false;
					Door.BlocksMovement = false;

					Animator animator = Door.GetComponent<Animator>();

					if (animator != null)
					{
						animator.SetBool("IsOpen", true);
					}
				}
			}
		}

		public override void Undo()
		{
			base.Undo();

			Inventory inventory = Actor.GetComponent<Inventory>();

			if (inventory != null)
			{
				Animator animator = Door.GetComponent<Animator>();

				if (animator != null)
				{
					animator.SetBool("IsOpen", false);
				}

				Door.BlocksLineOfSight = lastBlocksLineOfSight;
				Door.BlocksMovement = lastBlocksMovement;

				inventory.Add(RequiredKey);
			}
		}
	}
}
