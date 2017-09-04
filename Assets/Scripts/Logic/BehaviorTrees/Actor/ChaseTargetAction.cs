using AI.BehaviorTrees;
using InstaDungeon.Components;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class ChaseTargetAction : ActionNode
	{
		private string entityToChase;

		public ChaseTargetAction(string targetIdInBlackboard)
		{
			entityToChase = targetIdInBlackboard;
		}

		protected override NodeStates Tick(Tick tick)
		{
			NodeStates result = NodeStates.Failure;
			MapManager mapManager = Locator.Get<MapManager>();
			Entity chaser = tick.Target as Entity;
			Entity target = null;

			if (tick.Blackboard.TryGet(entityToChase, out target) && target != null)
			{
				int2[] path = mapManager.GetPath(chaser.CellTransform.Position, target.CellTransform.Position);

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
