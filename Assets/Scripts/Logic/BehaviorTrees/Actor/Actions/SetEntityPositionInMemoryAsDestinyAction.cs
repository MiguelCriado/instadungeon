using AI.BehaviorTrees;
using InstaDungeon.Components;
using System;
using System.Collections.Generic;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class SetEntityPositionInMemoryAsDestinyAction : ActionNode
	{
		private string memoryId;
		private string destinyId;
		private Predicate<Entity> entityMatch;

		public SetEntityPositionInMemoryAsDestinyAction(string entitiesMemoryId, string destinyIdInMemory, Predicate<Entity> entityMatch)
		{
			memoryId = entitiesMemoryId;
			destinyId = destinyIdInMemory;
			this.entityMatch = entityMatch;
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
						if (entityMatch(entitiesEnumerator.Current))
						{
							tick.Blackboard.Set(destinyId, entitiesEnumerator.Current.CellTransform.Position);
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
