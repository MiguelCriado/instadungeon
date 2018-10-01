namespace AI.BehaviorTrees
{
	public class Sequence : CompositeNode
	{
		public Sequence(params BaseNode[] children) : base(children)
		{
		}

		protected override NodeStates Tick(Tick tick)
		{
			NodeStates result;

			for (int i = 0; i < children.Length; i++)
			{
				result = children[i].Execute(tick);

				if (result != NodeStates.Success)
				{
					return result;
				}
			}

			return NodeStates.Success;
		}
	}
}
