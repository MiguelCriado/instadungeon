using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InstaDungeon.MapGeneration
{
	[Serializable]
	public class HilbertLayoutGenerator : BaseLayoutGenerator<HilbertLayoutGeneratorSettings, HilbertLayoutLevelSettings>
	{
		[Flags]
		private enum Connections
		{
			None = 0,
			North = 1,
			East = 2,
			South = 4,
			West = 8
		}

		private static readonly int2[] DIRS = new[]
		{
		new int2(0, 1),
		new int2(1, 0),
		new int2(0, -1),
		new int2(-1, 0),
		};

		public int2 Entrance { get { return initialHilbertTile; } }
		public int2 Exit { get { return exit; } }

		private int height;
		private int width;
		private int zoneWidth;
		private int zoneHeight;

		private int2 offsetFrame;
		private int n;
		private int2 initialHilbertTile;
		private int2 exit;
		private Connections[,] layoutConnections;

		public HilbertLayoutGenerator(HilbertLayoutGeneratorSettings settings) : base(settings)
		{

		}

		public override Layout NewLayout(int level)
		{
			Layout result = new Layout();

			return result;
		}


		public override string ToString()
		{
			return "Hilbert";
		}

		public override Layout Iterate(Layout layout, int level)
		{
			SetLevelSettings(level);

			Layout result = layout;
			Connections[,] layoutArray = GenerateLayoutArray();
			AddZones(result, layoutArray);
			ConnectZones(result, layoutArray);
			result.InitialZone = result.FindZoneByPosition(new int2(Entrance.x * zoneWidth, Entrance.y * zoneHeight));
			result.FinalZone = result.FindZoneByPosition(new int2(Exit.x * zoneWidth, Exit.y * zoneHeight));
			return result;
		}

		public override bool IsDone(Layout layout, int level)
		{
			return layout.Zones.Count > 0;
		}

		private void SetLevelSettings(int level)
		{
			HilbertLayoutLevelSettings levelSettings = settings.GetSettings(level);

			width = levelSettings.Dimensions.x;
			height = levelSettings.Dimensions.y;
			zoneWidth = levelSettings.ZoneDimensions.x;
			zoneHeight = levelSettings.ZoneDimensions.y;
		}

		private void AddZones(Layout layout, Connections[,] layoutArray)
		{
			for (int i = 0; i < layoutArray.GetLength(0); i++)
			{
				for (int j = 0; j < layoutArray.GetLength(1); j++)
				{
					Zone zone = new Zone();
					zone.bounds = new RectangleInt(i * zoneWidth, j * zoneHeight, zoneWidth, zoneHeight);
					layout.Add(zone);
				}
			}
		}

		private void ConnectZones(Layout result, Connections[,] layoutArray)
		{
			for (int i = 0; i < layoutArray.GetLength(0); i++)
			{
				for (int j = 0; j < layoutArray.GetLength(1); j++)
				{
					Zone currentZone = result.FindZoneByPosition(new int2(i * zoneWidth, j * zoneHeight));

					if (i >= 0 && (layoutArray[i, j] & Connections.East) == Connections.East)
					{
						Zone otherZone = result.FindZoneByPosition(new int2((i + 1) * zoneWidth, j * zoneHeight));

						if (result.GetAdjacentZones(currentZone).FindByValue(otherZone) == null)
						{
							result.ConnectZones(currentZone, otherZone);
						}
					}

					if (i <= width - 1 && (layoutArray[i, j] & Connections.West) == Connections.West)
					{
						Zone otherZone = result.FindZoneByPosition(new int2((i - 1) * zoneWidth, j * zoneHeight));

						if (result.GetAdjacentZones(currentZone).FindByValue(otherZone) == null)
						{
							result.ConnectZones(currentZone, otherZone);
						}
					}

					if (j >= 0 && (layoutArray[i, j] & Connections.North) == Connections.North)
					{
						Zone otherZone = result.FindZoneByPosition(new int2(i * zoneWidth, (j + 1) * zoneHeight));

						if (result.GetAdjacentZones(currentZone).FindByValue(otherZone) == null)
						{
							result.ConnectZones(currentZone, otherZone);
						}
					}

					if (j <= height - 1 && (layoutArray[i, j] & Connections.South) == Connections.South)
					{
						Zone otherZone = result.FindZoneByPosition(new int2(i * zoneWidth, (j - 1) * zoneHeight));

						if (result.GetAdjacentZones(currentZone).FindByValue(otherZone) == null)
						{
							result.ConnectZones(currentZone, otherZone);
						}
					}
				}
			}
		}

		private Connections[,] GenerateLayoutArray()
		{
			Initialize();
			GenerateLayout();
			CleanUpLayout();
			initialHilbertTile = new int2(Hilbert2Layout(initialHilbertTile).x, Hilbert2Layout(initialHilbertTile).y);
			exit = new int2(Hilbert2Layout(exit).x, Hilbert2Layout(exit).y);
			return layoutConnections;
		}

		private void Initialize()
		{
			layoutConnections = new Connections[width, height];
			InitializeConnections();
			InitializeMembers();
			initialHilbertTile = FindEntrance();
		}

		private void InitializeConnections()
		{
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					layoutConnections[i, j] = Connections.None;
				}
			}
		}

		private void InitializeMembers()
		{
			n = Math.Max(8, NextPowerOf2(Math.Max(height, width)));
			offsetFrame = SetOffset();
		}

		private int2 SetOffset()
		{
			int2 result = new int2(Random.Range(0, n - width + 1), Random.Range(0, n - height + 1));
			return result;
		}

		private int2 FindEntrance()
		{
			int2 result = int2.zero;
			bool found = false;
			int i = 0;
			int max = n * n - 1;

			while (!found && i <= max)
			{
				int2 candidate = HilbertCurve.d2xy(n, i);

				if (IsInsideOffsetFrame(candidate.x, candidate.y))
				{
					result = candidate;
					found = true;
				}

				i++;
			}
			return result;
		}

		private void GenerateLayout()
		{
			int initialD = HilbertCurve.xy2d(n, initialHilbertTile.x, initialHilbertTile.y);
			int i = initialD;
			int maxSteps = n * n - 1;
			Connections direction = Connections.None;
			List<List<int2>> unconnectedPaths = new List<List<int2>>();
			List<int2> currentPath = new List<int2>();
			int2 currentTile = new int2(initialHilbertTile.x, initialHilbertTile.y);
			currentPath.Add(Hilbert2Layout(currentTile));

			while (i < maxSteps)
			{
				int2 nextTile = HilbertCurve.d2xy(n, i + 1);

				if (IsInsideOffsetFrame(nextTile.x, nextTile.y))
				{
					direction = GetDirection(currentTile, nextTile);

					if (direction != Connections.None) // we can connect currentTile and nextTile directly. 
					{
						int2 layoutTile = Hilbert2Layout(currentTile);
						layoutConnections[layoutTile.x, layoutTile.y] |= direction;
					}
					else
					{
						unconnectedPaths.Add(currentPath);
						currentPath = new List<int2>();
					}

					currentPath.Add(Hilbert2Layout(nextTile));
					currentTile = nextTile;
				}

				i++;
			}

			unconnectedPaths.Add(currentPath);
			ConnectUnconnectedPaths(unconnectedPaths);
			exit = currentTile;
		}

		private void ConnectUnconnectedPaths(List<List<int2>> unconnectedPaths)
		{
			int numPaths = unconnectedPaths.Count;
			bool connected = true;
			bool connectedToNext = true;

			for (int i = numPaths - 1; i >= 0; i--)
			{
				if (!connected || !connectedToNext)
				{
					connectedToNext = ConnectToNextPaths(unconnectedPaths, connectedToNext, i);
					// if connectedToNext == false, we failed trying to connect the next path. 
					// We have to try on the next iteration again
				}

				if (i > 0)
				{
					connected = false;
					connected = ConnectToPreviousPaths(unconnectedPaths, connected, i);
					// if connected == false, we failed trying to connect the previous path. 
					// We have to try on the next iteration with connectToNext. 
				}
			}
		}

		private bool ConnectToNextPaths(List<List<int2>> unconnectedPaths, bool connectedToNext, int i)
		{
			connectedToNext = false;
			int j = i + 2;

			while (!connectedToNext && j < unconnectedPaths.Count)
			{
				int k = unconnectedPaths[i].Count - 1;

				while (!connectedToNext && k >= 0)
				{
					int m = 0;

					while (!connectedToNext && m < unconnectedPaths[j].Count)
					{
						Connections direction = GetDirection(unconnectedPaths[i][k], unconnectedPaths[j][m]);

						if (direction != Connections.None)
						{
							layoutConnections[unconnectedPaths[i][k].x, unconnectedPaths[i][k].y] |= direction;
							connectedToNext = true;
						}

						m++;
					}

					k--;
				}

				j++;
			}
			return connectedToNext;
		}

		private bool ConnectToPreviousPaths(List<List<int2>> unconnectedPaths, bool connected, int i)
		{
			int j = i - 1;

			while (!connected && j >= 0)
			{
				int k = 0;

				while (!connected && k < unconnectedPaths[i].Count)
				{
					int m = unconnectedPaths[j].Count - 1;

					while (!connected && m >= 0)
					{
						Connections direction = GetDirection(unconnectedPaths[i][k], unconnectedPaths[j][m]);

						if (direction != Connections.None)
						{
							layoutConnections[unconnectedPaths[i][k].x, unconnectedPaths[i][k].y] |= direction;
							connected = true;
						}

						m--;
					}

					k++;
				}

				j--;
			}

			return connected;
		}

		private void CleanUpLayout()
		{
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					if (i > 0
						&& (layoutConnections[i - 1, j] & Connections.East) == Connections.East)
					{
						layoutConnections[i, j] |= Connections.West;
					}

					if (i < width - 1
						&& (layoutConnections[i + 1, j] & Connections.West) == Connections.West)
					{
						layoutConnections[i, j] |= Connections.East;
					}

					if (j > 0
						&& (layoutConnections[i, j - 1] & Connections.North) == Connections.North)
					{
						layoutConnections[i, j] |= Connections.South;
					}

					if (j < height - 1
						&& (layoutConnections[i, j + 1] & Connections.South) == Connections.South)
					{
						layoutConnections[i, j] |= Connections.North;
					}
				}
			}
		}

		private Connections GetDirection(int2 origin, int2 destiny)
		{
			Connections result = Connections.None;

			if (origin + DIRS[0] == destiny)
			{
				result = Connections.North;
			}
			else if (origin + DIRS[1] == destiny)
			{
				result = Connections.East;
			}
			else if (origin + DIRS[2] == destiny)
			{
				result = Connections.South;
			}
			else if (origin + DIRS[3] == destiny)
			{
				result = Connections.West;
			}
			return result;
		}

		private bool IsInsideOffsetFrame(int x, int y)
		{
			bool result = (x >= offsetFrame.x
						   && x < offsetFrame.x + width
						   && y >= offsetFrame.y
						   && y < offsetFrame.y + height);
			return result;
		}

		private int NextPowerOf2(int x)
		{
			int result = -1;

			if (x >= 0)
			{
				bool found = false;
				int i = 0;

				while (!found)
				{
					if (Mathf.Pow(2, i) > x)
					{
						result = (int)Mathf.Pow(2, i);
						found = true;
					}

					i++;
				}
			}

			return result;
		}

		private int2 Hilbert2Layout(int2 point)
		{
			int2 result = new int2(point.x - offsetFrame.x, point.y - offsetFrame.y);
			return result;
		}

		private void PaintLayout()
		{
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					PaintConnections(layoutConnections[i, j], i, j);
				}
			}
		}

		private void PaintConnections(Connections cell, int x, int y)
		{
			if ((cell & Connections.North) == Connections.North)
			{
				Debug.DrawLine(new Vector2(x, y), new Vector2(x, y + 1));
			}

			if ((cell & Connections.East) == Connections.East)
			{
				Debug.DrawLine(new Vector2(x, y), new Vector2(x + 1, y));
			}

			if ((cell & Connections.South) == Connections.South)
			{
				Debug.DrawLine(new Vector2(x, y), new Vector2(x, y - 1));
			}

			if ((cell & Connections.West) == Connections.West)
			{
				Debug.DrawLine(new Vector2(x, y), new Vector2(x - 1, y));
			}
		}
	}
}
