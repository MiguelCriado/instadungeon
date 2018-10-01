using InstaDungeon.Actions;
using InstaDungeon.Components;
using UnityEngine;

namespace InstaDungeon
{
	[CreateAssetMenu(menuName = "InstaDungeon/Interactions/AttackInteraction", fileName = "new AttackInteraction", order = 1000)]
	public class AttackInteraction : Interaction
	{
		public override void EntityEntersRange(Entity activeActor, Entity pasiveActor)
		{
			// TODO ? 
		}

		public override void EntityExitsRange(Entity activeActor, Entity pasiveActor)
		{
			// TODO ? 
		}

		public override IAction Interact(Entity activeActor, Entity pasiveActor)
		{
			AttackAction result = new AttackAction(activeActor, pasiveActor);
			return result;
		}

		protected override bool IsValidInteractionInternal(Entity activeActor, Entity pasiveActor)
		{
			return AttackAction.IsValidInteraction(activeActor, pasiveActor);
		}
	}
}
