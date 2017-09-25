using InstaDungeon.Actions;
using InstaDungeon.Components;
using InstaDungeon.Configuration;
using UnityEngine;

namespace InstaDungeon
{
	[CreateAssetMenu(menuName = "InstaDungeon/Interactions/TrapDoorInteraction", fileName = "new TrapDoorInteraction", order = 1000)]
	public class TrapDoorInteraction : Interaction
	{
		public ItemInfo RequiredItem { get { return requiredItem; } }

		[SerializeField] private ItemInfo requiredItem;

		public override IAction Interact(Entity activeActor, Entity pasiveActor)
		{
			OpenTrapDoorAction result = new OpenTrapDoorAction(activeActor, pasiveActor, requiredItem);
			return result;
		}

		protected override bool IsValidInteractionInternal(Entity activeActor, Entity pasiveActor)
		{
			Inventory inventory = activeActor.GetComponent<Inventory>();
			return inventory != null && inventory.BagContains(requiredItem);
		}
	}
}
