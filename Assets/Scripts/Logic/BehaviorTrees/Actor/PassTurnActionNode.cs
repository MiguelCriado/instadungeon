using AI.BehaviorTrees;
using InstaDungeon.Components;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class PassTurnActionNode : ActionNode
	{
		protected override NodeStates Tick(Tick tick)
		{
			Entity target = tick.Target as Entity;
			Actor actor = target.GetComponent<Actor>();
			actor.PassTurn();

			return NodeStates.Success;
		}
	}
}
