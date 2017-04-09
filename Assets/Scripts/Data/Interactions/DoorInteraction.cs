using InstaDungeon.Components;
using InstaDungeon.Configuration;
using UnityEngine;

namespace InstaDungeon
{
	[CreateAssetMenu(menuName = "InstaDungeon/Interactions/DoorInteraction", fileName = "new DoorInteraction", order = 1000)]
	public class DoorInteraction : Interaction
	{
		public ItemInfo RequiredKey { get { return requiredKey; } }

		[SerializeField] protected ItemInfo requiredKey;

		public override bool IsValidInteraction(Entity activeActor, Entity pasiveActor)
		{
			bool result = pasiveActor.BlocksMovement;

			if (result == true)
			{
				result = activeActor.GetComponent<Inventory>() != null;
			}

			return result;
		}

		public override void Interact(Entity activeActor, Entity pasiveActor)
		{
			Inventory inventory = activeActor.GetComponent<Inventory>();

			if (inventory != null)
			{
				if (inventory.Contains(requiredKey))
				{
					inventory.Remove(requiredKey);

					pasiveActor.BlocksLineOfSight = false;
					pasiveActor.BlocksMovement = false;

					Animator animator = pasiveActor.GetComponent<Animator>();

					if (animator != null)
					{
						animator.SetBool("IsOpen", true);
					}
				}
			}
		}
	}
}
