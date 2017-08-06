using AI.BehaviorTrees;
using InstaDungeon.Components;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class IsLevelCompletedCondition : ConditionNode
	{
		protected override NodeStates Tick(Tick tick)
		{
			NodeStates result = NodeStates.Failure;
			Entity player = GameManager.Player;
			CellTransform playerTransform = player.CellTransform;

			Cell playerCell = GameManager.MapManager[playerTransform.Position.x, playerTransform.Position.y];

			if 
			(
				playerCell != null
				&& playerCell.Prop != null
				&& playerCell.Prop.Info != null
				&& playerCell.Prop.Info.NameId == "Stairs Exit"
			)
			{
				TrapDoor trapDoor = playerCell.Prop.GetComponent<TrapDoor>();

				if (trapDoor != null && trapDoor.IsOpen)
				{
					UnityEngine.Debug.Log("Level finished!!!");
					result = NodeStates.Success;
				}
			}

			return result;
		}
	}
}
