using AI.BehaviorTrees;
using InstaDungeon.Components;
using UnityEngine;

namespace InstaDungeon.AI
{
	public abstract class AIBrain : ScriptableObject
	{
		private static BehaviorTree Tree;

		public void Think(Entity target, Blackboard blackboard)
		{
			GetTree().Tick(target, blackboard);
		}

		protected abstract BehaviorTree GenerateNewTree();

		private BehaviorTree GetTree()
		{
			if (Tree == null)
			{
				Tree = GenerateNewTree();
			}

			return Tree;
		}
	}
}
