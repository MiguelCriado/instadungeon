using AI.BehaviorTrees;
using System;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class CheckVariableCondition<T> : ConditionNode
	{
		private string variableId;
		private Predicate<T> match;

		public CheckVariableCondition(string variableIdInMemory, Predicate<T> conditionToMet)
		{
			variableId = variableIdInMemory;
			match = conditionToMet;
		}

		protected override NodeStates Tick(Tick tick)
		{
			NodeStates result = NodeStates.Failure;

			T variable;

			if (tick.Blackboard.TryGet(variableId, out variable))
			{
				if (match(variable) == true)
				{
					result = NodeStates.Success;
				}
				else
				{
					result = NodeStates.Failure;
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
