using AI.BehaviorTrees;
using InstaDungeon.Components;
using System.Collections.Generic;
using System.Text;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class RefreshVisibilityThresholdAction : ActionNode
	{
		private static readonly int2[] Directions = new int2[]
		{
			new int2(0, 1),
			new int2(1, 0),
			new int2(0, -1),
			new int2(-1, 0)
		};

		private struct Segment
		{
			public int StartX, EndX, Y;
			public sbyte Dir; // -1:above the previous segment, 1:below the previous segment, 0:no previous segment
			public bool ScanLeft, ScanRight;

			public Segment(int startX, int endX, int y, sbyte dir, bool scanLeft, bool scanRight)
			{
				StartX = startX;
				EndX = endX;
				Y = y;
				Dir = dir;
				ScanLeft = scanLeft;
				ScanRight = scanRight;
			}
		}

		private string thresholdId;
		private string checkedTilesId;

		public RefreshVisibilityThresholdAction(string thresholdIdInMemory, string checkedTilesIdInMemory)
		{
			thresholdId = thresholdIdInMemory;
			checkedTilesId = checkedTilesIdInMemory;
		}

		protected override NodeStates Tick(Tick tick)
		{
			NodeStates result = NodeStates.Success;
			Entity entity = tick.Target as Entity;
			HashSet<int2> threshold;
			HashSet<int2> checkedTiles;

			if (tick.Blackboard.TryGet(thresholdId, out threshold) && tick.Blackboard.TryGet(checkedTilesId, out checkedTiles))
			{
				ExpandThreshold(threshold, checkedTiles);
			}
			else
			{
				InitializeThreshold(entity.CellTransform.Position, tick);
			}

			return result;
		}

		private static void ExpandThreshold(HashSet<int2> threshold, HashSet<int2> checkedTiles)
		{
			MapManager mapManager = Locator.Get<MapManager>();
			HashSet<int2> newElements = new HashSet<int2>();
			HashSet<int2> elementsToRemove = new HashSet<int2>();
			var enumerator = threshold.GetEnumerator();

			while (enumerator.MoveNext())
			{
				Cell cell = mapManager[enumerator.Current.x, enumerator.Current.y];

				if (cell.Visibility != VisibilityType.Obscured)
				{
					elementsToRemove.Add(enumerator.Current);

					if (cell.TileInfo.TileType == TileType.Floor)
					{
						ExpandTile(newElements, checkedTiles, mapManager, enumerator.Current);
					}
				}
			}

			threshold.ExceptWith(elementsToRemove);
			threshold.UnionWith(newElements);
		}

		private static void ExpandTile(HashSet<int2> threshold, HashSet<int2> checkedTiles, MapManager mapManager, int2 tile)
		{
			for (int i = 0; i < Directions.Length; i++)
			{
				int2 tileToCheck = tile + Directions[i];

				if (!checkedTiles.Contains(tileToCheck) && Test(threshold, checkedTiles, mapManager, tileToCheck.x, tileToCheck.y))
				{
					ExpandTile(threshold, checkedTiles, mapManager, tileToCheck);
				}
			}
		}

		private static string HashSetToString(HashSet<int2> hashset)
		{
			StringBuilder result = new StringBuilder();
			var enumerator = hashset.GetEnumerator();

			result.Append("(");

			while (enumerator.MoveNext())
			{
				result.Append(enumerator.Current.ToString());
				result.Append(", ");
			}

			if (hashset.Count > 0)
			{
				result.Remove(result.Length - 2, 2);
			}

			result.Append(")");
			return result.ToString();
		}

		private void InitializeThreshold(int2 origin, Tick tick)
		{
			HashSet<int2> threshold = new HashSet<int2>();
			HashSet<int2> checkedTiles = new HashSet<int2>();
			threshold.Add(origin);
			ExpandThreshold(threshold, checkedTiles);
			tick.Blackboard.Set(thresholdId, threshold);
			tick.Blackboard.Set(checkedTilesId, checkedTiles);
		}

		private static bool Test(HashSet<int2> threshold, HashSet<int2> checkedTiles, MapManager mapManager, int x, int y)
		{
			bool result = false;
			Cell cell = mapManager[x, y];

			if (cell != null)
			{
				if (cell.Visibility == VisibilityType.Obscured)
				{
					if (cell.TileInfo.TileType == TileType.Floor || cell.TileInfo.TileType == TileType.Wall)
					{
						threshold.Add(new int2(x, y));
						checkedTiles.Add(new int2(x, y));
					}
				}
				else
				{
					if (cell.TileInfo.TileType == TileType.Floor)
					{
						checkedTiles.Add(new int2(x, y));
						result = true;
					}
				}
			}

			return result;
		}
	}
}
