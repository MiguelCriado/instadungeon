using AI.BehaviorTrees;

public class LoadNewLevelAction : ActionNode
{
	protected override NodeStates Tick(Tick tick)
	{
		GameManager.LoadNewMap();

		return NodeStates.Success;
	}
}
