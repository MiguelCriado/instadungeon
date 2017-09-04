using AI.BehaviorTrees;
using InstaDungeon.Components;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class GoToLastKnownTargetPositionAction : ActionNode
	{
		private const string defaultStoredPositionId = "targetLastKnownPosition";

		private string storedPositionId;

		public GoToLastKnownTargetPositionAction(string storedPositionIdInBlackboard = defaultStoredPositionId)
		{
			storedPositionId = storedPositionIdInBlackboard;
		}

		protected override NodeStates Tick(Tick tick)
		{
			NodeStates result = NodeStates.Failure;
			int2 lastKnownPosition;

			if (tick.Blackboard.TryGet(storedPositionId, out lastKnownPosition))
			{
				MapManager mapManager = Locator.Get<MapManager>();
				Entity chaser = tick.Target as Entity;

				int2[] path = mapManager.GetPath(chaser.CellTransform.Position, lastKnownPosition);

				if (path.Length > 0)
				{
					Actor chaserActor = chaser.GetComponent<Actor>();
					int2 nextStep = path[0];
					int2 currentPosition = chaser.CellTransform.Position;

					if (nextStep.y > currentPosition.y)
					{
						chaserActor.Up();
					}
					else if (nextStep.x > currentPosition.x)
					{
						chaserActor.Right();
					}
					else if (nextStep.y < currentPosition.y)
					{
						chaserActor.Down();
					}
					else
					{
						chaserActor.Left();
					}

					result = NodeStates.Success;
				}
			}

			return result;
		}
	}
}
