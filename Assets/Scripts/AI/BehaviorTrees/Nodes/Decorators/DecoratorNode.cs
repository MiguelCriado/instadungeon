namespace AI.BehaviorTrees
{
	public abstract class DecoratorNode : BaseNode
	{
		protected BaseNode child;

		public DecoratorNode(BaseNode child)
		{
			this.child = child;
		}
	}
}
