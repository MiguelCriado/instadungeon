using AI.BehaviorTrees;
using InstaDungeon.Components;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class CanSeeEntityCondition : ConditionNode
	{
		private Entity entity;

		public CanSeeEntityCondition(Entity entity)
		{
			this.entity = entity;
		}

		protected override NodeStates Tick(Tick tick)
		{
			NodeStates result = NodeStates.Success;
			TileMap<Cell> map = Locator.Get<MapManager>().Map;

			IDTools.PlotFunction isClear = (int x, int y) =>
			{
				bool plotResult = true;
				Cell cell = map[x, y];

				if (cell == null || cell.BreaksLineOfSight())
				{
					plotResult = false;
					result = NodeStates.Failure;
				}

				return plotResult;
			};

			Entity target = tick.Target as Entity;

			int2 actorPosition = target.CellTransform.Position;
			int2 entityPosition = entity.CellTransform.Position;

			IDTools.Line(actorPosition.x, actorPosition.y, entityPosition.x, entityPosition.y, isClear);

			return result;
		}
	}
}
