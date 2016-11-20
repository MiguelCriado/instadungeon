namespace AI.BehaviorTrees
{
	public class Priority : CompositeNode
	{
		public Priority(params BaseNode[] children) : base(children)
		{
		}

		protected override NodeStates Tick(Tick tick)
		{
			NodeStates result;

			for (int i = 0; i < children.Length; i++)
			{
				result = children[i].Execute(tick);

				if (result != NodeStates.Failure)
				{
					return result;
				}
			}

			return NodeStates.Failure;
		}
	}
}
