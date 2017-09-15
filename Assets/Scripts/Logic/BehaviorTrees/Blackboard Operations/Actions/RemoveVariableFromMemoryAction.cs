using AI.BehaviorTrees;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class RemoveVariableFromMemoryAction : ActionNode
	{
		private string variableId;

		public RemoveVariableFromMemoryAction(string variableIdInMemory)
		{
			variableId = variableIdInMemory;
		}

		protected override NodeStates Tick(Tick tick)
		{
			tick.Blackboard.Remove(variableId);
			return NodeStates.Success;
		}
	}
}
