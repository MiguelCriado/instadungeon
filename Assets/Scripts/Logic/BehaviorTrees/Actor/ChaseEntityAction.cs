using AI.BehaviorTrees;
using InstaDungeon.Components;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class ChaseEntityAction : ActionNode
	{
		private Entity entityToChase;

		public ChaseEntityAction(Entity entityToChase)
		{
			this.entityToChase = entityToChase;
		}

		protected override NodeStates Tick(Tick tick)
		{
			MapManager mapManager = Locator.Get<MapManager>();
			Entity chaser = tick.Target as Entity;

			int2[] path = mapManager.GetPath(chaser.CellTransform.Position, entityToChase.CellTransform.Position);

			if (path.Length > 1)
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

				return NodeStates.Success;
			}
			else
			{
				return NodeStates.Failure;
			}
		}
	}
}
