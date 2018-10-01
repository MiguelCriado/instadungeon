namespace AI.BehaviorTrees
{
	public class Repeat : DecoratorNode
	{
		private static readonly string REPEAT_TIMES = "repeatTimes";

		private int times;

		public Repeat(int times, BaseNode child) : base(child)
		{
			this.times = times;
		}

		protected override void Open(Tick tick)
		{
			base.Open(tick);

			tick.Blackboard.Set(REPEAT_TIMES, 0, tick.Tree.Id, Id);
		}

		protected override NodeStates Tick(Tick tick)
		{
			NodeStates result = child.Execute(tick);

			if (result == NodeStates.Error)
			{
				return result;
			}

			int repeatTimes;
			tick.Blackboard.TryGet(REPEAT_TIMES, tick.Tree.Id, Id, out repeatTimes);

			if (repeatTimes++ >= times)
			{
				return NodeStates.Success;
			}
			else
			{
				tick.Blackboard.Set(REPEAT_TIMES, repeatTimes, tick.Tree.Id, Id);

				return NodeStates.Running;
			}
		}
	}
}
