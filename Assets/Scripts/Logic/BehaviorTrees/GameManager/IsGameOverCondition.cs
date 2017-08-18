﻿using AI.BehaviorTrees;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class IsGameOverCondition : ConditionNode
	{
		protected override NodeStates Tick(Tick tick)
		{
			NodeStates result = NodeStates.Failure;

			if (GameManager.GameState == GameState.GameOver)
			{
				result = NodeStates.Success;
			}

			return result;
		}
	}
}
