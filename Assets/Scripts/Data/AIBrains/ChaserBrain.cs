using AI.BehaviorTrees;
using InstaDungeon.BehaviorTreeNodes;
using InstaDungeon.Components;
using UnityEngine;

namespace InstaDungeon.AI
{
	[CreateAssetMenu(menuName = "InstaDungeon/AI Brains/ChaserBrain", fileName = "new ChaserBrain", order = 1000)]
	public class ChaserBrain : AIBrain
	{
		private static BehaviorTree Tree;

		public override void CreateTree()
		{
			if (Tree == null)
			{
				Tree = new BehaviorTree
				(
					new Priority
					(
						new Sequence
						(
							new CanSeeEntityCondition(GameManager.Player),
							new ChaseEntityAction(GameManager.Player)
						),
						new PassTurnActionNode()
					)
				);
			}
		}

		public override void Think(Entity target, Blackboard blackboard)
		{
			Tree.Tick(target, blackboard);
		}
	}
}
