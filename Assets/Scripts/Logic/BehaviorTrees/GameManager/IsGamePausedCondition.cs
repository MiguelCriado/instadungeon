using AI.BehaviorTrees;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class IsGamePausedCondition : ConditionNode
	{
		protected override NodeStates Tick(Tick tick)
		{
			NodeStates result = NodeStates.Failure;

			if (GameManager.GameState == GameState.Paused)
			{
				result = NodeStates.Success;
			}

			return result;
		}
	}
}
