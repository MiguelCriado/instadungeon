namespace AI.BehaviorTrees
{
	public class MemSequence : CompositeNode
	{
		private static readonly string RUNNING_CHILD = "runningChild";

		public MemSequence(params BaseNode[] children) : base(children)
		{
		}

		protected override void Open(Tick tick)
		{
			base.Open(tick);

			tick.Blackboard.Set(RUNNING_CHILD, 0, tick.Tree.Id, Id);
		}

		protected override NodeStates Tick(Tick tick)
		{
			NodeStates result;

			int child;
			tick.Blackboard.TryGet(RUNNING_CHILD, tick.Tree.Id, Id, out child);

			for (int i = child; i < children.Length; i++)
			{
				result = children[i].Execute(tick);

				if (result != NodeStates.Success)
				{
					if (result == NodeStates.Running)
					{
						tick.Blackboard.Set(RUNNING_CHILD, i, tick.Tree.Id, Id);
					}

					return result;
				}
			}

			return NodeStates.Success;
		}
	}
}
