using System.Collections.Generic;

namespace AI.BehaviorTrees
{
	public class Tick
	{
		public BehaviorTree Tree { get; set; }
		public object Target { get; set; }
		public Blackboard Blackboard { get; set; }
		public List<BaseNode> OpenNodes { get { return openNodes; } }
		public int NodeCount { get { return nodeCount; } }

		private List<BaseNode> openNodes;
		private int nodeCount;
		private bool debug;

		public Tick()
		{
			openNodes = new List<BaseNode>();
		}

		public void Initialize(BehaviorTree tree, object target, Blackboard blackboard)
		{
			nodeCount = 0;
			openNodes.Clear();
			Tree = tree;
			Target = target;
			Blackboard = blackboard;
		}

		public void EnterNode(BaseNode node)
		{
			nodeCount++;
			openNodes.Add(node);
			// TODO call debug
		}

		public void OpenNode(BaseNode node)
		{
			// TODO call debug
		}

		public void TickNode(BaseNode node)
		{
			// TODO call debug
		}

		public void CloseNode(BaseNode node)
		{
			if (openNodes.Count > 0)
			{
				openNodes.RemoveAt(openNodes.Count - 1);
			}
		}

		public void ExitNode(BaseNode node)
		{
			// TODO call debug
		}
	}
}
