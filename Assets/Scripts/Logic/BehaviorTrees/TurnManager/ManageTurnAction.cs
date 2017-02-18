using AI.BehaviorTrees;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class ManageTurnAction : ActionNode
	{
		private TurnManager manager;

		protected override void Open(Tick tick)
		{
			base.Open(tick);

			manager = tick.Target as TurnManager;
			manager.UpdateTurn();
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
				manager.Update();

				return NodeStates.Running;
			}
		}
	}
}
