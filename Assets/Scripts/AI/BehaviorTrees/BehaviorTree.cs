using System;
using System.Collections.Generic;

namespace AI.BehaviorTrees
{
	public class BehaviorTree
	{
		private static readonly string NODE_COUNT = "nodeCount";

		public string Id { get; private set; }

		private BaseNode root;
		private Tick tick;


		public BehaviorTree(BaseNode root)
		{
			Id = Guid.NewGuid().ToString();
			this.root = root;
			tick = new Tick();
		}

		public void Tick(object target, Blackboard blackboard)
		{
			tick.Initialize(this, target, blackboard);

			root.Execute(tick);

			List<BaseNode> lastOpenNodes;
			blackboard.TryGet(Blackboard.OPEN_NODES, Id, out lastOpenNodes);
			List<BaseNode> currentOpenNodes = tick.OpenNodes;

			int start = 0;

			for (int i = 0; i < Math.Min(lastOpenNodes.Count, currentOpenNodes.Count); i++)
			{
				start = i + 1;

				if (lastOpenNodes[i] != currentOpenNodes[i])
				{
					break;
				}
			}

			for (int i = lastOpenNodes.Count - 1; i >= start; i--)
			{
				lastOpenNodes[i].CloseWrapper(tick);
			}

			blackboard.Set(Blackboard.OPEN_NODES, currentOpenNodes, Id);
			blackboard.Set(NODE_COUNT, tick.NodeCount, Id);
		}
	}
}
