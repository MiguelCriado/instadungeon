using AI.BehaviorTrees;
using InstaDungeon.Components;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class IsLevelCompletedCondition : ConditionNode
	{
		protected override NodeStates Tick(Tick tick)
		{
			Entity player = GameManager.Player;
			CellTransform playerTransform = player.GetComponent<CellTransform>();

			Cell playerCell = GameManager.MapManager[playerTransform.Position.x, playerTransform.Position.y];

			if (playerCell != null && playerCell.TileInfo.TileType == TileType.Exit)
			{
				UnityEngine.Debug.Log("Level finished!!!");
				return NodeStates.Success;
			}
			else
			{
				return NodeStates.Failure;
			}
		}
	}
}
