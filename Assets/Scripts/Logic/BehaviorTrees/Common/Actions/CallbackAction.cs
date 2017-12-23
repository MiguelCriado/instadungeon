using AI.BehaviorTrees;
using System;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class CallbackAction : ActionNode
	{
		private Action callback;

		public CallbackAction(Action callback)
		{
			this.callback = callback;
		}

		protected override NodeStates Tick(Tick tick)
		{
			NodeStates result = NodeStates.Error;

			if (callback != null)
			{
				callback();
				result = NodeStates.Success;
			}

			return result;
		}
	}
}
