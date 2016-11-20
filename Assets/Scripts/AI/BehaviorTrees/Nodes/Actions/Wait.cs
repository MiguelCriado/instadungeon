using System;

namespace AI.BehaviorTrees
{
	public class Wait : ActionNode
	{
		private static readonly string START_TIME = "startTime";

		private long endTime;

		public Wait(long milliseconds)
		{
			endTime = milliseconds;
		}

		protected override void Open(Tick tick)
		{
			base.Open(tick);

			DateTime startTime = DateTime.Now;
			tick.Blackboard.Set(START_TIME, startTime, tick.Tree.Id, Id);
		}

		protected override NodeStates Tick(Tick tick)
		{
			DateTime currentTime = DateTime.Now;
			DateTime startTime;
			tick.Blackboard.TryGet(START_TIME, tick.Tree.Id, Id, out startTime);

			if ((currentTime - startTime).TotalMilliseconds > endTime)
			{
				return NodeStates.Success;
			}

			return NodeStates.Running;
		}
	}
}
