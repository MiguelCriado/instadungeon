using System.Collections.Generic;

public static class TileMapper
{
	public class TileLayerInfo
	{
		public int Layer;
		public Tile Tile;

		public TileLayerInfo(int layer, Tile tile)
		{
			Layer = layer;
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
					result.Add(new TileLayerInfo(0, tileSet.GetTile(9)));
				break;
				case TileType.Wall:
					result = new List<TileLayerInfo>();
					AddLowerWallTile(result, map, position, tileSet);
					result.Add(new TileLayerInfo(1, tileSet.GetTile("wall", GetNeighboursMask(map, position, TileType.Wall))));
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
				case CenterWall: tileIndex = 26; break;
				case RightWall: tileIndex = 27; break;
			}
		}

		if (tileIndex != uint.MaxValue)
		{
			result.Add(new TileLayerInfo(0, tileSet.GetTile(tileIndex)));
		}
	}
}
