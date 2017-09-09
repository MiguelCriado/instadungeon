using AI.BehaviorTrees;
using InstaDungeon.Components;
using System.Collections.Generic;
using System.Text;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class RefreshVisibilityThreshold : ActionNode
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

		public RefreshVisibilityThreshold(string thresholdIdInMemory, string checkedTilesIdInMemory)
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
			SearchForThreshold(threshold, checkedTiles, origin.x, origin.y);
			tick.Blackboard.Set(thresholdId, threshold);
			tick.Blackboard.Set(checkedTilesId, checkedTiles);
		}

		private static void SearchForThreshold(HashSet<int2> threshold, HashSet<int2> checkedTiles, int x, int y)
		{
			MapManager mapManager = Locator.Get<MapManager>();

			if (!Test(threshold, checkedTiles, mapManager, x, y))
			{
				return;
			}

			Stack<Segment> stack = new Stack<Segment>();
			stack.Push(new Segment(x, x + 1, y, 0, true, true));

			while (stack.Count > 0)
			{
				Segment segment = stack.Pop();
				int startX = segment.StartX, endX = segment.EndX;

				if (segment.ScanLeft) // if we should extend the segment towards the left...
				{
					while (Test(threshold, checkedTiles, mapManager, startX - 1, segment.Y))
					{
						startX--;
					}
					
				}
				if (segment.ScanRight)
				{
					while (Test(threshold, checkedTiles, mapManager, endX, segment.Y))
					{
						endX++;
					}
				}

				// at this point, the segment from startX (inclusive) to endX (exclusive) is filled. compute the region to ignore
				segment.StartX--; // since the segment is bounded on either side by filled cells or array edges, we can extend the size of
				segment.EndX++;   // the region that we're going to ignore in the adjacent lines by one
				// scan above and below the segment and add any new segments we find

				if (mapManager[x, segment.Y + 1] != null)
				{
					AddLine(threshold, checkedTiles, mapManager, stack, startX, endX, segment.Y + 1, segment.StartX, segment.EndX, -1, segment.Dir <= 0);
				}

				if (mapManager[x, segment.Y - 1] != null)
				{
					AddLine(threshold, checkedTiles, mapManager, stack, startX, endX, segment.Y - 1, segment.StartX, segment.EndX, 1, segment.Dir >= 0);
				}
			}
		}

		private static void AddLine(HashSet<int2> threshold, HashSet<int2> checkedTiles, MapManager mapManager, Stack<Segment> stack, int startX, int endX, int y, int ignoreStart, int ignoreEnd, sbyte dir, bool isNextInDir)
		{
			int regionStart = int.MinValue, x;

			for (x = startX; x < endX; x++) // scan the width of the parent segment
			{
				if ((isNextInDir || x < ignoreStart || x >= ignoreEnd) && Test(threshold, checkedTiles, mapManager, x, y))	// if we're outside the region we
				{																								// should ignore and the cell is clear
					if (regionStart == int.MinValue)
					{
						regionStart = x; // and start a new segment if we haven't already
					}
				}
				else if (regionStart > int.MinValue) // otherwise, if we shouldn't fill this cell and we have a current segment...
				{
					stack.Push(new Segment(regionStart, x, y, dir, regionStart == startX, false)); // push the segment
					regionStart = int.MinValue; // and end it
				}

				if (!isNextInDir && x < ignoreEnd && x >= ignoreStart)
				{
					x = ignoreEnd - 1; // skip over the ignored region
				}
			}

			if (regionStart > int.MinValue)
			{
				stack.Push(new Segment(regionStart, x, y, dir, regionStart == startX, true));
			}
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

				//if (cell.TileInfo.TileType == TileType.Floor)
				//{
				//	checkedTiles.Add(new int2(x, y));

				//	if (cell.Visibility == VisibilityType.Obscured)
				//	{
				//		threshold.Add(new int2(x, y));
				//	}
				//	else
				//	{
				//		result = true;
				//	}
				//}
			}

			return result;
		}
	}
}
