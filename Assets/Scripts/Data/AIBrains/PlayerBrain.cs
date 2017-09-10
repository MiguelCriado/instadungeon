using AI.BehaviorTrees;
using InstaDungeon.BehaviorTreeNodes;
using InstaDungeon.Components;
using UnityEngine;

namespace InstaDungeon.AI
{
	[CreateAssetMenu(menuName = "InstaDungeon/AI Brains/PlayerBrain", fileName = "new PlayerBrain", order = 1000)]
	public class PlayerBrain : AIBrain
	{
		private static readonly string ThresholdId = "Threshold";
		private static readonly string CheckedTilesId = "CheckedTilesId";
		private static readonly string TileId = "ThresholdTileId";
		private static readonly string ItemsMemoryId = "ItemsMemoryId";
		private static readonly string PropsMemoryId = "PropsMemoryId";
		private static readonly string SilverKeyNameId = "Key Silver";

		protected override BehaviorTree GenerateNewTree()
		{
			return new BehaviorTree
			(
				new Sequence
				(
					RecognizeEnvironment(),
					Act()
				)
			);
		}

		private BaseNode RecognizeEnvironment()
		{
			return new Sequence
			(
				new RefreshVisibilityThresholdAction(ThresholdId, CheckedTilesId),
				new MemorizeEntitiesInFloorAction(EntityTypeInMap.Item, ItemsMemoryId),
				new MemorizeEntitiesInFloorAction(EntityTypeInMap.Prop, PropsMemoryId)
			);
		}

		private BaseNode Act()
		{
			return new Priority
			(
				// ExitFloor(),
				Explore()
			);
		}

		private BaseNode ExitFloor()
		{
			return new Priority
			(
				new MemSequence
				(
					new Inverter(new InventoryContainsCondition(SilverKeyNameId, InventorySlotType.Bag))
					// TODO Condition: knows position of key
					// TODO set key's position as destiny
					// TODO go to destiny
				),
				new MemSequence
				(
					// TODO Condition: knows position of StairsExit
					// TODO set StairsExit's position as destiny
					// TODO go to destiny
				)
			);
		}

		private BaseNode Explore()
		{
			return new Sequence
			(
				new PickClosestRandomTileInThresholdAction(TileId, ThresholdId),
				new GoToStoredPositionAction(TileId)
			);
		}
	}
}
