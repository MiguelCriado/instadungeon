using AI.BehaviorTrees;
using InstaDungeon.Components;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class StoreLastKnownEntityPositionAction : ActionNode
	{
		private Entity entity;

		public StoreLastKnownEntityPositionAction(Entity entity)
		{
			this.entity = entity;
		}

		protected override NodeStates Tick(Tick tick)
		{
			int2 playerPosition = entity.CellTransform.Position;
			tick.Blackboard.Set("targetLastKnownPosition", playerPosition);

			return NodeStates.Success;
		}
	}
}
