using AI.BehaviorTrees;
using InstaDungeon.Components;
using System.Collections.Generic;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class EntitiesMemoryContainsCondition : ConditionNode
	{
		private string memoryId;
		private string entityId;

		public EntitiesMemoryContainsCondition(string entitiesMemoryId, string entityInfoNameId)
		{
			memoryId = entitiesMemoryId;
			entityId = entityInfoNameId;
		}

		protected override NodeStates Tick(Tick tick)
		{
			NodeStates result = NodeStates.Failure;
			Dictionary<int2, HashSet<Entity>> memory;

			if (tick.Blackboard.TryGet(memoryId, out memory))
			{
				var enumerator = memory.Values.GetEnumerator();

				while (result == NodeStates.Failure && enumerator.MoveNext())
				{
					var entitiesEnumerator = enumerator.Current.GetEnumerator();

					while (result == NodeStates.Failure && entitiesEnumerator.MoveNext())
					{
						if (entitiesEnumerator.Current.Info.NameId == entityId)
						{
							result = NodeStates.Success;
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
