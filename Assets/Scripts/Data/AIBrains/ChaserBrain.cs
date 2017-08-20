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
				GameManager gameManager = Locator.Get<GameManager>();

				Tree = new BehaviorTree
				(
					new Priority
					(
						new Sequence
						(
							new CanSeeEntityCondition(gameManager.Player),
							new StoreLastKnownEntityPositionAction(gameManager.Player),
							new ChaseEntityAction(gameManager.Player)
						),
						new GoToLastKnownPositionAction(),
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
