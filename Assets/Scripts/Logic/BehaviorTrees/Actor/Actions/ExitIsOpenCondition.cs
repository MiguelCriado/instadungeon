using AI.BehaviorTrees;
using InstaDungeon.Components;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class ExitIsOpenCondition : ConditionNode
	{
		protected override NodeStates Tick(Tick tick)
		{
			NodeStates result = NodeStates.Failure;
			MapManager mapManager = Locator.Get<MapManager>();
			Entity exit = mapManager.Map[mapManager.Map.ExitPoint].Prop;
			TrapDoor trapDoor = exit.GetComponent<TrapDoor>();

			if (trapDoor != null && trapDoor.IsOpen)
			{
				result = NodeStates.Success;
			}

			return result;
		}
	}
}
