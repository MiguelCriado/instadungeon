using AI.BehaviorTrees;

public class IsLevelCompletedCondition : ConditionNode
{
	protected override NodeStates Tick(Tick tick)
	{
		return NodeStates.Failure; // TODO: actually check for level completion
	}
}
