using AI.BehaviorTrees;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class StoreVariableInMemoryAction<T> : ActionNode
	{
		public delegate T VariableSetter();

		public string variableId;
		public VariableSetter setter;

		public StoreVariableInMemoryAction(string variableIdInMemory, VariableSetter setter)
		{
			variableId = variableIdInMemory;
			this.setter = setter;
		}

		protected override NodeStates Tick(Tick tick)
		{
			tick.Blackboard.Set(variableId, setter);
			return NodeStates.Success;
		}
	}
}
