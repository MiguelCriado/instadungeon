using AI.BehaviorTrees;
using InstaDungeon.Components;
using System.Collections.Generic;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class CanSeeAnyEnemyCondition : ConditionNode
	{
		protected override NodeStates Tick(Tick tick)
		{
			NodeStates result = NodeStates.Error;
			Entity entity = tick.Target as Entity;

			if (entity != null)
			{
				SideManager sideManager = Locator.Get<SideManager>();
				SideComponent sideComponent = entity.GetComponent<SideComponent>();

				if (sideComponent != null)
				{
					TileMap<Cell> map = Locator.Get<MapManager>().Map;
					List<Entity> enemies = sideManager.GetOtherSidesActors(sideComponent.Side);
					result = NodeStates.Failure;
					int i = 0;

					while (result == NodeStates.Failure && i < enemies.Count)
					{
						bool isEnemyInVisibleCell = map[enemies[i].CellTransform.Position].Visibility == VisibilityType.Visible;

						if (isEnemyInVisibleCell && CanSeeEntity(entity, enemies[i]))
						{
							result = NodeStates.Success;
						}

						i++;
					}
				}
			}

			return result;
		}

		private bool CanSeeEntity(Entity origin, Entity target)
		{
			bool result = true;
			TileMap<Cell> map = Locator.Get<MapManager>().Map;

			IDTools.PlotFunction isClear = (int x, int y) =>
			{
				bool plotResult = true;
				Cell cell = map[x, y];

				if (cell == null || cell.BreaksLineOfSight())
				{
					plotResult = false;
					result = false;
				}

				return plotResult;
			};

			int2 actorPosition = target.CellTransform.Position;
			int2 entityPosition = origin.CellTransform.Position;

			IDTools.Line(actorPosition.x, actorPosition.y, entityPosition.x, entityPosition.y, isClear);

			return result;
		}
	}
}
