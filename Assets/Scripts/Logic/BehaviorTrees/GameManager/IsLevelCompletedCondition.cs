using AI.BehaviorTrees;

public class IsLevelCompletedCondition : ConditionNode
{
	protected override NodeStates Tick(Tick tick)
	{
		CellTransform playerTransform = GameManager.Instance.player;
		Cell playerCell = GameManager.GetCell(playerTransform.Position.x, playerTransform.Position.y);

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
