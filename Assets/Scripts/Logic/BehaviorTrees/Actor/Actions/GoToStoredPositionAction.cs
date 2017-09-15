using AI.BehaviorTrees;
using InstaDungeon.Components;
using System;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class GoToStoredPositionAction : ActionNode
	{
		private string storedPositionId;
		private string pathId;
		private string currentPathNodeId;

		public GoToStoredPositionAction(string storedPositionIdInMemory)
		{
			storedPositionId = storedPositionIdInMemory;
			pathId = string.Format("{0} - Path - {1}", storedPositionId, Guid.NewGuid());
			currentPathNodeId = string.Format("{0} - CurrentPathNode - {1}", storedPositionId, Guid.NewGuid());
		}

		protected override void Open(Tick tick)
		{
			int2 destiny;

			if (tick.Blackboard.TryGet(storedPositionId, out destiny))
			{
				MapManager mapManager = Locator.Get<MapManager>();
				Entity target = tick.Target as Entity;

				int2[] path = mapManager.GetPath(target.CellTransform.Position, destiny);
				tick.Blackboard.Set(pathId, path);
				tick.Blackboard.Set(currentPathNodeId, 0);
			}
		}

		protected override NodeStates Tick(Tick tick)
		{
			NodeStates result = NodeStates.Failure;
			int2[] path;
			int currentNode;

			if (tick.Blackboard.TryGet(pathId, out path) && tick.Blackboard.TryGet(currentPathNodeId, out currentNode))
			{
				if (path.Length > 0)
				{
					if (currentNode < path.Length)
					{
						Entity target = tick.Target as Entity;
						Actor targetActor = target.GetComponent<Actor>();
						int2 nextStep = path[currentNode];
						int2 currentPosition = target.CellTransform.Position;

						if (nextStep.y > currentPosition.y)
						{
							targetActor.Up();
						}
						else if (nextStep.x > currentPosition.x)
						{
							targetActor.Right();
						}
						else if (nextStep.y < currentPosition.y)
						{
							targetActor.Down();
						}
						else
						{
							targetActor.Left();
						}

						tick.Blackboard.Set(currentPathNodeId, ++currentNode);
						result = NodeStates.Running;
					}
					else
					{
						result = NodeStates.Success;
					}
				}
			}

			return result;
		}

		protected override void Close(Tick tick)
		{
			base.Close(tick);

			tick.Blackboard.Remove(pathId);
			tick.Blackboard.Remove(currentPathNodeId);
		}
	}
}
