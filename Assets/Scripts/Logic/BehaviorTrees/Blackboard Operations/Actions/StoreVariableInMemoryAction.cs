using AI.BehaviorTrees;
using System;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class StoreVariableInMemoryAction<T> : ActionNode
	{
		public string variableId;
		public Func<T> setter;

		public StoreVariableInMemoryAction(string variableIdInMemory, Func<T> setter)
		{
			variableId = variableIdInMemory;
			this.setter = setter;
		}

		protected override NodeStates Tick(Tick tick)
		{
			tick.Blackboard.Set(variableId, setter());
			return NodeStates.Success;
		}
	}
}
