namespace AI.BehaviorTrees
{
	public class Inverter : DecoratorNode
	{
		public Inverter(BaseNode child) : base(child)
		{
		}

		protected override NodeStates Tick(Tick tick)
		{
			NodeStates result;

			if (child == null)
			{
				return NodeStates.Error;
			}

			result = child.Execute(tick);

			if (result == NodeStates.Success)
			{
				result = NodeStates.Failure;
			}
			else if (result == NodeStates.Failure)
			{
				result = NodeStates.Success;
			}

			return result;
		}
	}
}
