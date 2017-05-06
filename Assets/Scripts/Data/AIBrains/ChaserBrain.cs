using AI.BehaviorTrees;
using InstaDungeon.Components;
using UnityEngine;

namespace InstaDungeon.AI
{
	[CreateAssetMenu(menuName = "InstaDungeon/AI Brains/ChaserBrain", fileName = "new ChaserBrain", order = 1000)]
	public class ChaserBrain : AIBrain
	{
		public override void Think(Entity context, Actor actor, Blackboard blackboard)
		{
			// TODO: chase the player
		}
	}
}
