using System.Collections.Generic;

namespace InstaDungeon
{
	public class TileMapWeightedGraph : IWeightedGraph<int2, int>
	{
		private TileMap<Cell> map;
		private int2 currentGoal;

		public TileMapWeightedGraph(TileMap<Cell> map)
		{
			this.map = map;
		}

		private static readonly int2[] Dirs = new[]
		{
			new int2(1, 0),
			new int2(0, -1),
			new int2(-1, 0),
			new int2(0, 1) 
        };

		public void SetGoal(int2 goal)
		{
			currentGoal = goal;
		}

		public int Cost(int2 a, int2 b)
		{
			// TODO: refine Cost calculation
			return 1;
		}

		public IEnumerable<int2> Neighbors(int2 id)
		{
			int2 nextPosition;
			Cell next;

			for (int i = 0; i < Dirs.Length; i++)
			{
				nextPosition = id + Dirs[i];
				next = map[nextPosition];

				if (next != null 
					&& (next.IsWalkable() || nextPosition == currentGoal))
				{
					yield return nextPosition;
				}
			}
		}
	}
}
