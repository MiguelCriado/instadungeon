using AI.BehaviorTrees;
using InstaDungeon.Components;
using System;
using System.Collections.Generic;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class IsEntityInSightRangeCondition : ConditionNode
	{
		private Predicate<Entity> entityMatch;
		private string entitiesMemoryId;
		private MapManager mapManager;

		public IsEntityInSightRangeCondition(string entitiesMemoryId, Predicate<Entity> entityMatch)
		{
			this.entityMatch = entityMatch;
			this.entitiesMemoryId = entitiesMemoryId;
			mapManager = Locator.Get<MapManager>();
		}

		protected override NodeStates Tick(Tick tick)
		{
			NodeStates result = NodeStates.Failure;
			Dictionary<int2, HashSet<Entity>> memory;

			if (tick.Blackboard.TryGet(entitiesMemoryId, out memory))
			{
				var memoryEnumerator = memory.Values.GetEnumerator();

				while (result == NodeStates.Failure && memoryEnumerator.MoveNext())
				{
					var entitiesEnumerator = memoryEnumerator.Current.GetEnumerator();

					while (result == NodeStates.Failure && entitiesEnumerator.MoveNext())
					{
						if (entityMatch(entitiesEnumerator.Current))
						{
							int2 position = entitiesEnumerator.Current.CellTransform.Position;

							if (mapManager[position.x, position.y].Visibility == VisibilityType.Visible)
							{
								result = NodeStates.Success;
							}
						}
					}
				}
			}
			else
			{
				result = NodeStates.Error;
			}

			return result;
		}
	}
}
