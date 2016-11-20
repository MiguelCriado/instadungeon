namespace AI.BehaviorTrees
{
	public class Parallel : CompositeNode
	{
		private int successThreshold;
		private int failureThreshold;

		public Parallel(int successThreshold, int failureThreshold, params BaseNode[] children) : base(children)
		{
			this.successThreshold = successThreshold;
			this.failureThreshold = failureThreshold;
		}

		protected override NodeStates Tick(Tick tick)
		{
			int successCount = 0;
			int failureCount = 0;
			NodeStates childResult;

			for (int i = 0; i < children.Length; i++)
			{
				childResult = children[i].Execute(tick);

				if (childResult == NodeStates.Success)
				{
					successCount++;
				}
				else if (childResult == NodeStates.Failure)
				{
					failureCount++;
				}
			}

			if (successCount >= successThreshold)
			{
				return NodeStates.Success;
			}
			else if (failureCount >= failureThreshold)
			{
				return NodeStates.Failure;
			}
			else
			{
				return NodeStates.Running;
			}
		}
	}
}
