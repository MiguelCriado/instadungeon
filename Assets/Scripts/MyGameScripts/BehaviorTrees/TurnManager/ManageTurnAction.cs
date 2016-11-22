using AI.BehaviorTrees;

public class ManageTurnAction : ActionNode
{
	private TurnManager manager;

	protected override void Open(Tick tick)
	{
		base.Open(tick);

		manager = tick.Target as TurnManager;
		manager.Update();
	}

	protected override NodeStates Tick(Tick tick)
	{
		manager = tick.Target as TurnManager;

		if (manager.TurnDone || !manager.Running)
		{
			return NodeStates.Success;
		}
		else
		{
			return NodeStates.Running;
		}
	}
}
