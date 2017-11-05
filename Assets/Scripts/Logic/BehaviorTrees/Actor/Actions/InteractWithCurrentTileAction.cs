using AI.BehaviorTrees;
using InstaDungeon.Components;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class InteractWithCurrentTileAction : ActionNode
	{
		protected override NodeStates Tick(Tick tick)
		{
			NodeStates result = NodeStates.Failure;
			Entity target = tick.Target as Entity;
			Actor actor = target.GetComponent<Actor>();

			if (actor != null)
			{
				actor.InteractWithCurrentTile();
				result = NodeStates.Success;
			}
			else
			{
				result = NodeStates.Error;
			}

			return result;
		}
	}
}
