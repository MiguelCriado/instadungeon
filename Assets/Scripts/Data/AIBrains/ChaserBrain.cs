using AI.BehaviorTrees;
using InstaDungeon.BehaviorTreeNodes;
using UnityEngine;

namespace InstaDungeon.AI
{
	[CreateAssetMenu(menuName = "InstaDungeon/AI Brains/ChaserBrain", fileName = "new ChaserBrain", order = 1000)]
	public class ChaserBrain : AIBrain
	{
		private static readonly string TargetId = "targetActor";
		private static readonly string LastKnownPositionId = "lastKnownPosition";

		private static BehaviorTree Tree;

		protected override BehaviorTree GetTree()
		{
			if (Tree == null)
			{
				Tree = GenerateNewTree();
			}

			return Tree;
		}

		protected override BehaviorTree GenerateNewTree()
		{
			return new BehaviorTree
			(
				new Priority
				(
					new Sequence
					(
						new AcquireClosestTargetAction(TargetId),
						new StoreLastKnownTargetPositionAction(TargetId, LastKnownPositionId),
						new ChaseTargetAction(TargetId)
					),
					new GoToLastKnownTargetPositionAction(LastKnownPositionId),
					new PassTurnActionAction()
				)
			);
		}
	}
}
