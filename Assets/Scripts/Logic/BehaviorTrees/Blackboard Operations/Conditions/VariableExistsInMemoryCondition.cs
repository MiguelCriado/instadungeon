using AI.BehaviorTrees;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class VariableExistsInMemoryCondition<T> : ConditionNode
	{
		private string variableId;

		public VariableExistsInMemoryCondition(string variableIdInMemory)
		{
			variableId = variableIdInMemory;
		}

		protected override NodeStates Tick(Tick tick)
		{
			NodeStates result = NodeStates.Failure;
			T variable;

			if (tick.Blackboard.TryGet(variableId, out variable))
			{
				result = NodeStates.Success;
			}

			return result;
		}
	}
}
