using AI.BehaviorTrees;
using System;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class FuncCondition : ConditionNode
	{
		private Func<bool> function;

		public FuncCondition(Func<bool> function)
		{
			this.function = function;
		}

		protected override NodeStates Tick(Tick tick)
		{
			NodeStates result = NodeStates.Error;

			try
			{
				if (function())
				{
					result = NodeStates.Success;
				}
				else
				{
					result = NodeStates.Failure;
				}
			}
			catch
			{
				result = NodeStates.Error;
			}

			return result;
		}
	}
}
