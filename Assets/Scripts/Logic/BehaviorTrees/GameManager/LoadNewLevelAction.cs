using AI.BehaviorTrees;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class LoadNewLevelAction : ActionNode
	{
		protected override NodeStates Tick(Tick tick)
		{
			Locator.Get<GameManager>().LoadNewMap();

			return NodeStates.Success;
		}
	}
}
