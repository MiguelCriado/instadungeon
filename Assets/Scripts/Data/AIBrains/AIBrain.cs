using AI.BehaviorTrees;
using InstaDungeon.Components;
using UnityEngine;

namespace InstaDungeon.AI
{
	public abstract class AIBrain : ScriptableObject
	{
		public abstract void Think(Entity context, Actor actor, Blackboard blackboard);
	}
}
