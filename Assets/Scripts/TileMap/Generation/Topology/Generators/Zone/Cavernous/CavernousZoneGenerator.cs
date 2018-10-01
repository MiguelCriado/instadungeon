﻿using KDTree;
using System.Collections.Generic;
using UnityEngine;

namespace InstaDungeon.MapGeneration
{
	[System.Serializable]
	public class CavernousZoneGenerator : BaseZoneGenerator<CavernousZoneGeneratorSettings, CavernousZoneLevelSettings>
	{
		/// <summary>
		/// Convenient inner class to store lists of cells. Tipically containing a cavern system. 
		/// </summary>
		private struct CellList
		{
			public int id;
			public List<Vector2> cells;

			public CellList(int id, List<Vector2> cells)
			{
				this.id = id;
				this.cells = cells;
			}

			public static int CompareByListLength(CellList item1, CellList item2)
			{
				return item1.cells.Count.CompareTo(item2.cells.Count);
			}
		}

		public delegate char PlaceWallRule(int x, int y);

		private readonly static int EMPTY = 0;
		private readonly static int WALL = 1;
		private readonly static int FIXED_FLOOR = 2;
		private readonly static int FLOOR = 3;

		private float initialWallProb;
		private int iterations;
		private int refiningIterations;

		private int height;
		private int width;
		private int[,] zoneArray;
		private List<int2> fixedFloor;

		public CavernousZoneGenerator(CavernousZoneGeneratorSettings settings) : base(settings)
		{

		}

		#region [PUBLIC API]

		public void SetLevelSettings(int level)
		{
			CavernousZoneLevelSettings levelSettings = settings.GetSettings(level);

			initialWallProb = levelSettings.InitialWallProbability;
			iterations = levelSettings.Iterations;
			refiningIterations = levelSettings.RefiningIterations;
		}

		public override TileMap<TileType> PreConnectZones(TileMap<TileType> map, int level)
		{
			SetLevelSettings(level);
			List<Zone> zonesToConnect = new List<Zone>();

			if (map.Layout.Zones.Count > 0)
			{
				zonesToConnect.Add(map.Layout.InitialZone);

				while (zonesToConnect.Count > 0)
				{
					Zone zone = zonesToConnect[0];
					zonesToConnect.Remove(zone);

					NodeList<Zone> neighbors = map.Layout.GetAdjacentZones(zone);

					if (neighbors != null)
					{
						Zone neighbor;

						for (int i = 0; i < neighbors.Count; i++)
						{
							neighbor = neighbors[i].Value;

							if (!zone.connections.ContainsValue(neighbor))
							{
								int2 connectionPoint;
								List<int2> connectionCandidates = zone.bounds.ContactArea(neighbor.bounds, true);
								connectionPoint = connectionCandidates[Random.Range(0, connectionCandidates.Count - 1)];
								zone.AddConnectionPoint(connectionPoint, neighbor);

								int2 contactPoint;

								if (neighbor.ContactPoint(connectionPoint, out contactPoint))
								{
									neighbor.AddConnectionPoint(contactPoint, zone);
									zonesToConnect.Add(neighbor);
								}
							}
						}
					}
				}
			}

			return map;
		}

		public override TileMap<TileType> Generate(TileMap<TileType> map, int level)
		{
			SetLevelSettings(level);
			TileMap<TileType> result = map;

			NodeList<Zone> zones = map.Layout.Zones.Nodes;

			// TODO: make this concurrent with threads

			for (int i = 0; i < zones.Count; i++)
			{
				result = Generate(zones[i].Value, result);
			}

			return result;
		}

		public override TileMap<TileType> PostConnectZones(TileMap<TileType> map, int level)
		{
			return map;
		}

		public override int2 GetSpawnPoint(TileMap<TileType> map, int level)
		{
			return FindPlaceForStairs(map, map.Layout.InitialZone);
		}

		public override int2 GetExitPoint(TileMap<TileType> map, int level)
		{
			return FindPlaceForStairs(map, map.Layout.FinalZone);
		}

		public override string ToString()
		{
			return "Cavernous";
		}

		#endregion

		#region [Helpers]

		private static int2 FindPlaceForStairs(TileMap<TileType> map, Zone zone)
		{
			int2 result = int2.zero;
			int currentSurroundingFloorTiles = -1;
			int targetSurroundingFloorTiles = 8;

			foreach (int2 tile in zone.tiles)
			{
				int thisTileSurroundingFloorTiles = CountSurroundingFloor(map, zone, tile);

				if (map.GetTile(tile.x, tile.y) == TileType.Floor && thisTileSurroundingFloorTiles >= currentSurroundingFloorTiles)
				{
					result = tile;
					currentSurroundingFloorTiles = thisTileSurroundingFloorTiles;

					if (currentSurroundingFloorTiles >= targetSurroundingFloorTiles)
					{
						break;
					}
				}
			}

			return result;
		}

		private static int CountSurroundingFloor(TileMap<TileType> map, Zone zone, int2 tile)
		{
			int result = 0;

			int2[] dirs = new[]
			{
			new int2(0, 1),
			new int2(1, 0),
			new int2(0, -1),
			new int2(-1, 0),
			new int2(-1, -1),
			new int2(-1, 1),
			new int2(1, 1),
			new int2(1, -1),
		};

			for (int i = 0; i < dirs.Length; i++)
			{
				int2 adjacentTile = tile + dirs[i];

				if (zone.tiles.Contains(tile + dirs[i]) && map.GetTile(adjacentTile.x, adjacentTile.y) == TileType.Floor)
				{
					result++;
				}
			}

			return result;
		}

		private TileMap<TileType> Generate(Zone zone, TileMap<TileType> map)
		{
			fixedFloor = new List<int2>();

			var connectionsEnumerator = zone.connections.GetEnumerator();

			while (connectionsEnumerator.MoveNext())
			{
				fixedFloor.Add(connectionsEnumerator.Current.Key - zone.bounds.Position);
			}

			Dictionary<int2, TileType> zoneTiles = Generate(zone.bounds.width, zone.bounds.height, zone.bounds.Position);

			var enumerator = zoneTiles.GetEnumerator();

			int2 position;
			TileType tile;

			while (enumerator.MoveNext())
			{
				position = enumerator.Current.Key;
				tile = enumerator.Current.Value;

				map[position.x, position.y] = tile;

				zone.tiles.Add(position);
			}

			return map;
		}

		private Dictionary<int2, TileType> Generate(int width, int height, int2 offset)
		{
			Dictionary<int2, TileType> result = new Dictionary<int2, TileType>();

			this.width = width;
			this.height = height;

			int[,] map = Generate();

			for (int i = 0; i < map.GetLength(0); i++)
			{
				for (int j = 0; j < map.GetLength(1); j++)
				{
					if (map[i, j] == WALL)
					{
						result.Add(new int2(i + offset.x, j + offset.y), TileType.Wall);
					}
					else if (map[i, j] >= FIXED_FLOOR)
					{
						result.Add(new int2(i + offset.x, j + offset.y), TileType.Floor);
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Generates a new Shape;
		/// </summary>
		/// <returns>The Shape generated</returns>
		private int[,] Generate()
		{
			InitializeShape();

			for (int i = 0; i < iterations; i++)
			{
				zoneArray = Step(1, 1, 5, 2, 2, 2);
			}

			for (int i = 0; i < refiningIterations; i++)
			{
				zoneArray = Step(1, 1, 5, 2, 2, -1);
			}

			IdentifyCaverns();
			ConnectCaverns();
			CleanUpShape();

			return zoneArray;
		}

		/// <summary>
		/// Initializes the shape with input parameters. 
		/// </summary>
		private void InitializeShape()
		{
			float wallCount = 0f;
			zoneArray = new int[width, height];

			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					if (Random.Range(0f, 1f) <= initialWallProb)
					{
						zoneArray[i, j] = WALL;
						wallCount++;
					}
					else
					{
						zoneArray[i, j] = FLOOR;
					}
				}
			}

			if (fixedFloor != null)
			{
				for (int i = 0; i < fixedFloor.Count; i++)
				{
					zoneArray[fixedFloor[i].x, fixedFloor[i].y] = FIXED_FLOOR;
				}
			}
		}

		/// <summary>
		/// Iterates once over the shape applying the cellular automata rules provided. 
		/// </summary>
		/// <param name="scopeX1"></param>
		/// <param name="scopeY1"></param>
		/// <param name="upperThreshold"></param>
		/// <param name="scopeX2"></param>
		/// <param name="scopeY2"></param>
		/// <param name="lowerThreshold"></param>
		/// <returns></returns>
		private int[,] Step(int scopeX1, int scopeY1, int upperThreshold, int scopeX2, int scopeY2, int lowerThreshold)
		{
			int[,] result = new int[width, height];

			for (int i = 0; i < zoneArray.GetLength(0); i++)
			{
				for (int j = 0; j < zoneArray.GetLength(1); j++)
				{
					result[i, j] = PlaceWallLogic(i, j, scopeX1, scopeY1, upperThreshold, scopeX2, scopeY2, lowerThreshold);
				}
			}

			return result;
		}

		/// <summary>
		/// Places a Wall tile or a Floor tile based on the cellular automata rule provided. 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="scopeX1"></param>
		/// <param name="scopeY1"></param>
		/// <param name="upperThreshold"></param>
		/// <param name="scopeX2"></param>
		/// <param name="scopeY2"></param>
		/// <param name="lowerThreshold"></param>
		/// <returns></returns>
		private int PlaceWallLogic(int x, int y, int scopeX1, int scopeY1, int upperThreshold, int scopeX2, int scopeY2, int lowerThreshold)
		{
			int result = FLOOR;

			if (zoneArray[x, y] != FIXED_FLOOR)
			{
				int numWalls1 = CountSurroundingWalls(x, y, scopeX1, scopeY1);
				int numWalls2 = CountSurroundingWalls(x, y, scopeX2, scopeY2);

				if (IsBound(x, y) ||
					(zoneArray[x, y] == WALL && (numWalls1 >= upperThreshold - 1 || numWalls2 <= lowerThreshold - 1)) ||
					(numWalls1 >= upperThreshold || numWalls2 <= lowerThreshold))
				{
					result = WALL;
				}
			}
			else
			{
				result = FIXED_FLOOR;
			}

			return result;
		}

		/// <summary>
		///  Counts how many walls are there around the tile described by [x, y] on the current Shape. 
		///  It scopes [scopeX, scopeY] tiles away from the tile we are checking.
		/// </summary>
		/// <param name="x"></param> x component of the tile we are checking. 
		/// <param name="y"></param> y component of the tile we are checking. 
		/// <param name="scopeX"></param> number of tiles away we are scoping horizontally. 
		/// <param name="scopeY"></param> number of tiles away we are scoping vertically. 
		/// <returns></returns>
		private int CountSurroundingWalls(int x, int y, int scopeX, int scopeY)
		{
			int result = 0;
			int xInit, yInit, xFinal, yFinal;

			xInit = x - scopeX;
			yInit = y - scopeY;
			xFinal = x + scopeX;
			yFinal = y + scopeY;

			for (int i = xInit; i < xFinal + 1; i++)
			{
				for (int j = yInit; j < yFinal + 1; j++)
				{
					if (IsWall(i, j))
					{
						result++;
					}
				}
			}

			return result;
		}

		private bool IsWall(int x, int y)
		{
			return IsOutOfBounds(x, y) || zoneArray[x, y] == WALL;
		}

		private bool IsOutOfBounds(int x, int y)
		{
			return x < 0 || x >= zoneArray.GetLength(0) || y < 0 || y >= zoneArray.GetLength(1);
		}

		private bool IsBound(int x, int y)
		{
			return x == 0 || y == 0 || x == zoneArray.GetLength(0) - 1 || y == zoneArray.GetLength(1) - 1;
		}

		private int[,] IdentifyCaverns()
		{
			int cont = FLOOR + 1;

			for (int i = 0; i < zoneArray.GetLength(0); i++)
			{
				for (int j = 0; j < zoneArray.GetLength(1); j++)
				{
					if (zoneArray[i, j] == FLOOR)
					{
						if (FloodFillIsle(i, j, FLOOR, cont) < 2)
						{
							FloodFillIsle(i, j, cont, WALL);
						}
						else
						{
							cont++;
						}
					}
					else if (zoneArray[i, j] == FIXED_FLOOR)
					{
						FloodFillIsle(i, j, FIXED_FLOOR, cont);
						cont++;
					}
				}
			}

			return zoneArray;
		}

		private int FloodFillIsle(int x, int y, int previousValue, int value)
		{
			zoneArray[x, y] = value;
			int cont = 1;

			if (!IsOutOfBounds(x, y + 1) && zoneArray[x, y + 1] == previousValue)
			{
				cont += FloodFillIsle(x, y + 1, previousValue, value);
			}

			if (!IsOutOfBounds(x + 1, y) && zoneArray[x + 1, y] == previousValue)
			{
				cont += FloodFillIsle(x + 1, y, previousValue, value);
			}

			if (!IsOutOfBounds(x, y - 1) && zoneArray[x, y - 1] == previousValue)
			{
				cont += FloodFillIsle(x, y - 1, previousValue, value);
			}

			if (!IsOutOfBounds(x - 1, y) && zoneArray[x - 1, y] == previousValue)
			{
				cont += FloodFillIsle(x - 1, y, previousValue, value);
			}

			return cont;
		}

		private List<CellList> ListCaverns()
		{
			List<CellList> result = new List<CellList>();
			CellList cavernList;

			for (int i = 0; i < zoneArray.GetLength(0); i++)
			{
				for (int j = 0; j < zoneArray.GetLength(1); j++)
				{
					if (zoneArray[i, j] > FLOOR)
					{
						if (!result.Exists(x => x.id == zoneArray[i, j]))
						{
							result.Add(new CellList(zoneArray[i, j], new List<Vector2>()));
						}

						cavernList = result.Find(x => x.id == zoneArray[i, j]);
						cavernList.cells.Add(new Vector2(i, j));
					}
				}
			}

			result.Sort(CellList.CompareByListLength);

			return result;
		}

		private void ConnectCaverns()
		{
			List<CellList> cavernList = ListCaverns();

			if (cavernList.Count > 1)
			{
				KDTree<Vector2> tree = new KDTree<Vector2>(2);
				FillTree(cavernList, ref tree);
				Vector2[] nearest;

				for (int i = 0; i < cavernList.Count - 1; i++)
				{
					nearest = FindNearestTile(tree, cavernList[i].cells);
					MakePath(nearest[0], nearest[1]);
				}
			}
		}

		private void MakePath(Vector2 origin, Vector2 destiny)
		{
			IDTools.PlotFunction plotter = PlacePathPoint;
			IDTools.Line((int)origin.x, (int)origin.y, (int)destiny.x, (int)destiny.y, plotter);
		}

		private bool PlacePathPoint(int x, int y)
		{
			bool result = true;

			if (!IsOutOfBounds(x, y) && !IsBound(x, y))
			{
				zoneArray[x, y] = FLOOR;
			}

			if (!IsOutOfBounds(x - 1, y) && !IsBound(x - 1, y))
			{
				zoneArray[x - 1, y] = FLOOR;
			}

			if (!IsOutOfBounds(x + 1, y) && !IsBound(x + 1, y))
			{
				zoneArray[x + 1, y] = FLOOR;
			}

			if (!IsOutOfBounds(x, y - 1) && !IsBound(x, y - 1))
			{
				zoneArray[x, y - 1] = FLOOR;
			}

			if (!IsOutOfBounds(x, y + 1) && !IsBound(x, y + 1))
			{
				zoneArray[x, y + 1] = FLOOR;
			}

			return result;
		}


		private static Vector2[] FindNearestTile(KDTree<Vector2> tree, List<Vector2> cells)
		{
			Vector2[] result = new Vector2[2];
			double nearestValue = float.PositiveInfinity;
			double[] point = new double[2];
			NearestNeighbour<Vector2> it;

			for (int i = 0; i < cells.Count; i++)
			{
				point[0] = cells[i].x;
				point[1] = cells[i].y;

				it = tree.NearestNeighbors(point, 1);

				while (it.MoveNext())
				{
					if (it.CurrentDistance < nearestValue)
					{
						nearestValue = it.CurrentDistance;
						result[0] = cells[i];
						result[1] = it.Current;
					}
				}
			}

			return result;
		}


		private static void FillTree(List<CellList> cavernList, ref KDTree<Vector2> tree)
		{
			CellList biggestCavern = cavernList[cavernList.Count - 1];

			for (int i = 0; i < biggestCavern.cells.Count; i++)
			{
				tree.AddPoint(new double[] { biggestCavern.cells[i].x, biggestCavern.cells[i].y }, biggestCavern.cells[i]);
			}
		}

		private void CleanUpShape()
		{
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					if (zoneArray[i, j] < FIXED_FLOOR)
					{
						if (IsNearAFloorTile(i, j, true))
						{
							zoneArray[i, j] = WALL;
						}
						else
						{
							zoneArray[i, j] = EMPTY;
						}
					}
					// TODO avoid fake passages on diagonals
				}
			}
		}

		private bool IsNearAFloorTile(int x, int y, bool checkDiagonals = false)
		{
			bool result = false;

			if ((!IsOutOfBounds(x - 1, y) && zoneArray[x - 1, y] >= FIXED_FLOOR)
				|| (!IsOutOfBounds(x + 1, y) && zoneArray[x + 1, y] >= FIXED_FLOOR)
				|| (!IsOutOfBounds(x, y - 1) && zoneArray[x, y - 1] >= FIXED_FLOOR)
				|| (!IsOutOfBounds(x, y + 1) && zoneArray[x, y + 1] >= FIXED_FLOOR))
			{
				result = true;
			}

			if
			(
				(checkDiagonals && !result)
				&&
				(
						(!IsOutOfBounds(x - 1, y + 1) && zoneArray[x - 1, y + 1] >= FIXED_FLOOR)
					|| (!IsOutOfBounds(x + 1, y + 1) && zoneArray[x + 1, y + 1] >= FIXED_FLOOR)
					|| (!IsOutOfBounds(x + 1, y - 1) && zoneArray[x + 1, y - 1] >= FIXED_FLOOR)
					|| (!IsOutOfBounds(x - 1, y - 1) && zoneArray[x - 1, y - 1] >= FIXED_FLOOR)
				)
			)
			{
				result = true;
			}

			return result;
		}

		#endregion
	}
}
