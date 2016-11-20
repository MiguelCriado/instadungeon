namespace AI.BehaviorTrees
{
	public abstract class CompositeNode : BaseNode
	{
		protected BaseNode[] children;

		public CompositeNode(params BaseNode[] children)
		{
			this.children = children;
		}
	}
}
