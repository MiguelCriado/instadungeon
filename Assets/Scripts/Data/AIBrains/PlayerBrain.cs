using AI.BehaviorTrees;
using InstaDungeon.BehaviorTreeNodes;
using UnityEngine;

namespace InstaDungeon.AI
{
	[CreateAssetMenu(menuName = "InstaDungeon/AI Brains/PlayerBrain", fileName = "new PlayerBrain", order = 1000)]
	public class PlayerBrain : AIBrain
	{
		private static readonly string ThresholdId = "Threshold";
		private static readonly string TileId = "ThresholdTileId";

		protected override BehaviorTree GenerateNewTree()
		{
			return new BehaviorTree
			(
				new Sequence
				(
					new RefreshVisibilityThreshold(ThresholdId),
					new Sequence // Exploration
					(
						new PickClosestRandomTileInThresholdAction(TileId, ThresholdId),
						new GoToStoredPositionNode(TileId)
					)
				)
			);
		}
	}
}
