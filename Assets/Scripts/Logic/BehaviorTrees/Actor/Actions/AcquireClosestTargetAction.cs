using AI.BehaviorTrees;
using InstaDungeon.Components;
using System.Collections.Generic;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class AcquireClosestTargetAction : ActionNode
	{
		private string targetId;

		public AcquireClosestTargetAction(string targetIdInBlackboard)
		{
			targetId = targetIdInBlackboard;
		}

		protected override NodeStates Tick(Tick tick)
		{
			NodeStates result = NodeStates.Success;
			Entity entity = tick.Target as Entity;
			SideComponent sideComponent = entity.GetComponent<SideComponent>();
			List<Entity> enemies = Locator.Get<SideManager>().GetOtherSidesActors(sideComponent.Side);
			Entity target = FindClosestEntity(enemies, entity);
			tick.Blackboard.Set(targetId, target);

			return result;
		}

		private Entity FindClosestEntity(List<Entity> entities, Entity origin)
		{
			Entity result = null;
			MapManager mapManager = Locator.Get<MapManager>();
			int minDistance = int.MaxValue;

			for (int i = 0; i < entities.Count; i++)
			{
				if (CanSeeEntity(origin, entities[i]))
				{
					int distance = mapManager.GetPath(origin.CellTransform.Position, entities[i].CellTransform.Position).Length;

					if (distance > 0 && distance < minDistance)
					{
						result = entities[i];
						minDistance = distance;
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
