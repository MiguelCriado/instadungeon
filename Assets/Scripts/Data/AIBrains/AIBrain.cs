using AI.BehaviorTrees;
using InstaDungeon.Components;
using UnityEngine;

namespace InstaDungeon.AI
{
	public abstract class AIBrain : ScriptableObject
	{
		public void Think(Entity target, Blackboard blackboard)
		{
			GetTree().Tick(target, blackboard);
		}

		protected abstract BehaviorTree GetTree();

		protected abstract BehaviorTree GenerateNewTree();
	}
}
