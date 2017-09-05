using AI.BehaviorTrees;
using InstaDungeon.Components;
using System.Collections.Generic;

namespace InstaDungeon.Actions
{
	public class RefreshVisibilityThreshold : ActionNode
	{
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

		private static readonly string ThresholdId = "visibilityThreshold";

		protected override NodeStates Tick(Tick tick)
		{
			NodeStates result = NodeStates.Success;
			Entity entity = tick.Target as Entity;
			HashSet<int2> threshold;

			if (tick.Blackboard.TryGet(ThresholdId, out threshold))
			{
				ExpandThreshold(threshold);
			}
			else
			{
				InitializeThreshold(entity.CellTransform.Position, tick);
			}

			return result;
		}

		private static void ExpandThreshold(HashSet<int2> threshold)
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

					Test(newElements, mapManager, enumerator.Current.x, enumerator.Current.y + 1);
					Test(newElements, mapManager, enumerator.Current.x + 1, enumerator.Current.y);
					Test(newElements, mapManager, enumerator.Current.x, enumerator.Current.y - 1);
					Test(newElements, mapManager, enumerator.Current.x - 1, enumerator.Current.y);
				}
			}

			threshold.ExceptWith(elementsToRemove);
			threshold.UnionWith(newElements);
		}

		private static void InitializeThreshold(int2 origin, Tick tick)
		{
			HashSet<int2> threshold = new HashSet<int2>();
			SearchForThreshold(threshold, origin.x, origin.y);
			tick.Blackboard.Set(ThresholdId, threshold);
		}

		private static void SearchForThreshold(HashSet<int2> threshold, int x, int y)
		{
			MapManager mapManager = Locator.Get<MapManager>();

			if (Test(threshold, mapManager, y, x))
			{
				return;
			}

			Stack<Segment> stack = new Stack<Segment>();
			stack.Push(new Segment(x, x + 1, y, 0, true, true));

			while (stack.Count >= 0)
			{
				Segment segment = stack.Pop();
				int startX = segment.StartX, endX = segment.EndX;

				if (segment.ScanLeft) // if we should extend the segment towards the left...
				{
					while (!Test(threshold, mapManager, segment.Y, startX - 1));
					
				}
				if (segment.ScanRight)
				{
					while (!Test(threshold, mapManager, segment.Y, endX));
				}

				// at this point, the segment from startX (inclusive) to endX (exclusive) is filled. compute the region to ignore
				segment.StartX--; // since the segment is bounded on either side by filled cells or array edges, we can extend the size of
				segment.EndX++;   // the region that we're going to ignore in the adjacent lines by one
				// scan above and below the segment and add any new segments we find

				if (mapManager[x, segment.Y - 1] != null)
				{
					AddLine(threshold, mapManager, stack, startX, endX, segment.Y - 1, segment.StartX, segment.EndX, -1, segment.Dir <= 0);
				}

				if (mapManager[x, segment.Y + 1] != null)
				{
					AddLine(threshold, mapManager, stack, startX, endX, segment.Y + 1, segment.StartX, segment.EndX, 1, segment.Dir >= 0);
				}
			}
		}

		private static void AddLine(HashSet<int2> threshold, MapManager mapManager, Stack<Segment> stack, int startX, int endX, int y, int ignoreStart, int ignoreEnd, sbyte dir, bool isNextInDir)
		{
			int regionStart = -1, x;

			for (x = startX; x < endX; x++) // scan the width of the parent segment
			{
				if ((isNextInDir || x < ignoreStart || x >= ignoreEnd) && !Test(threshold, mapManager, y, x)) // if we're outside the region we
				{                                                                            // should ignore and the cell is clear

					if (regionStart < 0)
					{
						regionStart = x; // and start a new segment if we haven't already
					}
				}
				else if (regionStart >= 0) // otherwise, if we shouldn't fill this cell and we have a current segment...
				{
					stack.Push(new Segment(regionStart, x, y, dir, regionStart == startX, false)); // push the segment
					regionStart = -1; // and end it
				}
				if (!isNextInDir && x < ignoreEnd && x >= ignoreStart)
				{
					x = ignoreEnd - 1; // skip over the ignored region
				}
			}

			if (regionStart >= 0)
			{
				stack.Push(new Segment(regionStart, x, y, dir, regionStart == startX, true));
			}
		}

		private static bool Test(HashSet<int2> threshold, MapManager mapManager, int y, int x)
		{
			bool result = false;
			Cell cell = mapManager[x, y];

			if (cell != null)
			{
				if (cell.Visibility == VisibilityType.Obscured)
				{
					threshold.Add(new int2(x, y));
					result = true;
				}
			}

			return result;
		}
	}
}
