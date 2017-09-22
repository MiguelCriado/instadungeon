using System.Collections.Generic;

namespace InstaDungeon
{
	public class TileMapIgnoreActorsWeightedGraph : TileMapWeightedGraph
	{
		public TileMapIgnoreActorsWeightedGraph(TileMap<Cell> map) : base(map)
		{

		}

		public override IEnumerable<int2> Neighbors(int2 id)
		{
			int2 nextPosition;
			Cell next;

			for (int i = 0; i < Dirs.Length; i++)
			{
				nextPosition = id + Dirs[i];
				next = map[nextPosition];

				if (next != null && (IsWalkable(next) || nextPosition == currentGoal))
				{
					yield return nextPosition;
				}
			}
		}

		private bool IsWalkable(Cell tile)
		{
			bool blocked = false;

			blocked |= !tile.TileInfo.Walkable	|| (tile.Prop != null && tile.Prop.BlocksMovement);

			if (blocked == false)
			{
				int i = 0;

				while (blocked == false && i < tile.Items.Count)
				{
					blocked |= tile.Items[i].BlocksMovement;
					i++;
				}
			}

			return !blocked;
		}
	}
}
