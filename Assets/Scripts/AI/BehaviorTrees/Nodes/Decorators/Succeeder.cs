namespace AI.BehaviorTrees
{
	public class Succeeder : DecoratorNode
	{
		public Succeeder(BaseNode child) : base(child)
		{
		}

		protected override NodeStates Tick(Tick tick)
		{
			if (child == null)
			{
				return NodeStates.Error;
			}

			child.Execute(tick);
			return NodeStates.Success;
		}
	}
}
