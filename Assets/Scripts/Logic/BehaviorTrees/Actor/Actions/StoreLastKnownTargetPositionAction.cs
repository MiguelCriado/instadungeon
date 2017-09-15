using AI.BehaviorTrees;
using InstaDungeon.Components;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class StoreLastKnownTargetPositionAction : ActionNode
	{
		private const string defaultStoredPositionId = "targetLastKnownPosition";

		private string targetId;
		private string storedPositionId;

		public StoreLastKnownTargetPositionAction(string targetIdInBlackboard, string storedPositionIdInBlackboard = defaultStoredPositionId)
		{
			targetId = targetIdInBlackboard;
			storedPositionId = storedPositionIdInBlackboard;
		}

		protected override NodeStates Tick(Tick tick)
		{
			NodeStates result = NodeStates.Failure;
			Entity target = null;

			if (tick.Blackboard.TryGet(targetId, out target) && target != null)
			{
				int2 targetPosition = target.CellTransform.Position;
				tick.Blackboard.Set(storedPositionId, targetPosition);

				result = NodeStates.Success;
			}

			return result;
		}
	}
}
