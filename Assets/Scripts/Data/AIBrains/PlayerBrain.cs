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
		private static readonly string SilverKeyItemId = "Key Silver";
		private static readonly string SilverKeyEntityId = "Silver Key";
		private static readonly string SilverKeyPositionId = "SilverKeyPositionId";
		private static readonly string CombatTargetId = "CombatTargetId";
		private static readonly string StairsExitEntityId = "Stairs Exit";
		private static readonly string StairsExitPositionId = "StairsExitPositionId";
		private static readonly string FloorLevelId = "FloorLevelId";

		private static BehaviorTree Tree;

		private GameManager gameManager;

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
			gameManager = Locator.Get<GameManager>();

			return new BehaviorTree
			(
				new Sequence
				(
					TidyMemory(),
					RecognizeEnvironment(),
					Act(),
					MemorizeCurrentFloor()
				)
			);
		}

		private BaseNode TidyMemory()
		{
			return new Succeeder
			(
				new Sequence
				(
					new VariableExistsInMemoryCondition<int>(FloorLevelId),
					new CheckVariableCondition<int>(FloorLevelId, (int x) => x != gameManager.CurrentFloor),
					new RemoveVariableFromMemoryAction(ThresholdId),
					new RemoveVariableFromMemoryAction(CheckedTilesId),
					new RemoveVariableFromMemoryAction(TileId),
					new RemoveVariableFromMemoryAction(ItemsMemoryId),
					new RemoveVariableFromMemoryAction(PropsMemoryId),
					new RemoveVariableFromMemoryAction(SilverKeyPositionId),
					new RemoveVariableFromMemoryAction(StairsExitPositionId),
					new RemoveVariableFromMemoryAction(CombatTargetId)
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
				Combat(),
				ExitFloor(),
				Explore()
			);
		}

		private BaseNode Combat()
		{
			return new Sequence
			(
				new CanSeeAnyEnemyCondition(),
				new AcquireClosestTargetAction(CombatTargetId),
				new ChaseTargetAction(CombatTargetId)
			);
		}

		private BaseNode ExitFloor()
		{
			return new Priority
			(
				new Sequence
				(
					new EntitiesMemoryContainsCondition(PropsMemoryId, StairsExitEntityId),
					new Parallel
					(
						1,
						2,
						new InventoryContainsCondition(SilverKeyItemId, InventorySlotType.Bag),
						new ExitIsOpenCondition()
					),
					new SetEntityPositionInMemoryAsDestinyAction(PropsMemoryId, StairsExitEntityId, StairsExitPositionId),
					new GoToStoredPositionAction(StairsExitPositionId, true)
				),
				new Sequence
				(
					new EntitiesMemoryContainsCondition(ItemsMemoryId, SilverKeyEntityId),
					new SetEntityPositionInMemoryAsDestinyAction(ItemsMemoryId, SilverKeyEntityId, SilverKeyPositionId),
					new GoToStoredPositionAction(SilverKeyPositionId, true)
				)
			);
		}

		private BaseNode Explore()
		{
			return new Sequence
			(
				new PickClosestRandomTileInThresholdAction(TileId, ThresholdId),
				new GetOneStepCloserToStoredPositionAction(TileId, true)
			);
		}

		private BaseNode MemorizeCurrentFloor()
		{
			return new StoreVariableInMemoryAction<int>(FloorLevelId, () => gameManager.CurrentFloor);
		}
	}
}
