using InstaDungeon.Actions;
using InstaDungeon.Components;
using UnityEngine;

namespace InstaDungeon
{
	[CreateAssetMenu(menuName = "InstaDungeon/Interactions/AttackInteraction", fileName = "new AttackInteraction", order = 1000)]
	public class AttackInteraction : Interaction
	{
		public override bool IsValidInteraction(Entity activeActor, Entity pasiveActor)
		{
			return AttackAction.IsValidInteraction(activeActor, pasiveActor);
		}

		public override IAction Interact(Entity activeActor, Entity pasiveActor)
		{
			AttackAction result = new AttackAction(activeActor, pasiveActor);
			return result;
		}
	}
}
