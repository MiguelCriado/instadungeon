using System.Collections.Generic;
using UnityEngine;

public static class TileMapper
{
	public class TileLayerInfo
	{
		public string SortingLayer;
		public int SortingOrder;
		public string Material;
		public Vector2 OffsetUnits;
		public Tile Tile;

		public TileLayerInfo(string sortingLayer, int sortingOrder, string material, Vector2 offsetUnits, Tile tile)
		{
			SortingLayer = sortingLayer;
			SortingOrder = sortingOrder;
			Material = material;
			OffsetUnits = offsetUnits;
			Tile = tile;
		}
	}

	public static readonly int2[] neighboursCheck = new int2[]
	{
		new int2(-1, 1),
		new int2(0, 1), 
		new int2(1, 1), 
		new int2(1, 0),
		new int2(1, -1),
		new int2(0, -1),
		new int2(-1, -1),
		new int2(-1, 0)
	};

	private static readonly uint[] FloorTiles =		new uint[]	{ 3, 4, 5,  9, 11, 15, 16, 17, 21 };
	private static readonly int[] FloorTilesWeight = new int[]	{ 0, 2, 3, 20, 1, 0, 0, 0, 0 };
	private static int FloorTilesTotalWeight;

	private static readonly uint[] WallTiles = new uint[] { 26, 28, 29, 30 };
	private static readonly int[] WallTilesWeight = new int[] { 20, 5, 1, 2 };
	private static int WallTilesTotalWeight;

	static TileMapper()
	{
		FloorTilesTotalWeight = 0;

		for (int i = 0; i < FloorTilesWeight.Length; i++)
		{
			FloorTilesTotalWeight += FloorTilesWeight[i];
		}

		WallTilesTotalWeight = 0;

		for (int i = 0; i < WallTilesWeight.Length; i++)
		{
			WallTilesTotalWeight += WallTilesWeight[i];
		}
	}

	public static List<TileLayerInfo> GetTileLayers(TileMap<Cell> map, int2 position, TileSet tileSet)
	{
		List<TileLayerInfo> result = null;
		Cell cell = map[position.x, position.y];

		if (cell != null)
		{
			TileInfo tile = cell.TileInfo;

			switch (tile.TileType)
			{
				default:
				case TileType.Space: break;
				case TileType.Floor:
					result = new List<TileLayerInfo>();
					result.Add(new TileLayerInfo("Bottom", -32768, "TileMap", Vector2.zero, tileSet.GetTile(GetRandomFloorTile())));
				break;
				case TileType.Wall:
					result = new List<TileLayerInfo>();
					AddLowerWallTile(result, map, position, tileSet);
					result.Add(new TileLayerInfo("Middle", -32768, "TileMap Occluder", Vector2.up, tileSet.GetTile("wall", GetNeighboursMask(map, position, TileType.Wall))));
				break;
			}
		}

		return result;
	}

	private static byte GetNeighboursMask(TileMap<Cell> map, int2 position, TileType tileType)
	{
		byte result = 0;
		byte mask = 128; // 10000000

		Cell neighbour;

		for (int i = 0; i < neighboursCheck.Length; i++)
		{
			neighbour = map[position.x + neighboursCheck[i].x, position.y + neighboursCheck[i].y];

			if (neighbour != null && neighbour.TileInfo != null && neighbour.TileInfo.TileType == tileType)
			{
				result += mask;
			}

			mask = (byte)(mask >> 1);
		}

		return result;
	}

	private const byte ThinWall = 4; // 0000100
	private const byte LeftWall = 6; // 0000110
	private const byte CenterWall = 14; // 001110
	private const byte RightWall = 12; // 0001100

	private static void AddLowerWallTile(List<TileLayerInfo> result, TileMap<Cell> map, int2 position, TileSet tileSet)
	{
		uint tileIndex = uint.MaxValue;
		Cell neighbour = map[position + int2.down];

		if (neighbour != null && neighbour.TileInfo.TileType == TileType.Floor)
		{
			byte tileMask = 4;
			neighbour = map[position + int2.left];

			if (neighbour != null && neighbour.TileInfo.TileType == TileType.Wall)
			{
				tileMask |= 8;
			}

			neighbour = map[position + int2.right];

			if (neighbour != null && neighbour.TileInfo.TileType == TileType.Wall)
			{
				tileMask |= 2;
			}

			switch (tileMask)
			{
				case ThinWall: tileIndex = 24; break;
				case LeftWall: tileIndex = 25; break;
				case CenterWall: tileIndex = GetRandomWallTile(); break;
				case RightWall: tileIndex = 27; break;
			}
		}

		if (tileIndex != uint.MaxValue)
		{
			result.Add(new TileLayerInfo("Bottom", -32768, "TileMap", Vector2.zero, tileSet.GetTile(tileIndex)));
		}
	}

	private static uint GetRandomFloorTile()
	{
		return PickRandomValue(FloorTilesTotalWeight, FloorTilesWeight, FloorTiles);
	}

	private static uint GetRandomWallTile()
	{
		return PickRandomValue(WallTilesTotalWeight, WallTilesWeight, WallTiles);
	}

	private static uint PickRandomValue(int totalWeight, int[] weightArray, uint[] valueArray)
	{
		uint result = uint.MaxValue;
		int cumulativeWeight = 0;
		int randomNumber = Random.Range(0, totalWeight);
		int i = 0;

		while (result == uint.MaxValue && i < weightArray.Length)
		{
			cumulativeWeight += weightArray[i];

			if (cumulativeWeight > randomNumber)
			{
				result = valueArray[i];
			}

			i++;
		}

		return result;
	}
}
