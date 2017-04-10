using InstaDungeon.Actions;
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

		public override IAction Interact(Entity activeActor, Entity pasiveActor)
		{
			OpenDoorAction result = new OpenDoorAction(activeActor, pasiveActor, requiredKey);
			return result;
		}
	}
}
