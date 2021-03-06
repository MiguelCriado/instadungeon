﻿using AI.BehaviorTrees;
using InstaDungeon.Components;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class GetOneStepCloserToStoredPositionAction : ActionNode
	{
		private string storedPositionId;
		private bool ignoreActors;

		public GetOneStepCloserToStoredPositionAction(string storedPositionIdInMemory, bool ignoreActors = false)
		{
			storedPositionId = storedPositionIdInMemory;
			this.ignoreActors = ignoreActors;
		}

		protected override NodeStates Tick(Tick tick)
		{
			NodeStates result = NodeStates.Failure;
			int2 destiny;

			if (tick.Blackboard.TryGet(storedPositionId, out destiny))
			{
				MapManager mapManager = Locator.Get<MapManager>();
				Entity target = tick.Target as Entity;
				int2[] path;

				if (ignoreActors == true)
				{
					path = mapManager.GetPathIgnoringActors(target.CellTransform.Position, destiny);
				}
				else
				{
					path = mapManager.GetPath(target.CellTransform.Position, destiny);
				}

				if (path.Length > 0)
				{
					Actor targetActor = target.GetComponent<Actor>();
					int2 nextStep = path[0];
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

					result = NodeStates.Success;
				}
			}

			return result;
		}
	}
}
