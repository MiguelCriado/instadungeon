﻿public static class TileMapper
{
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

	public static Tile GetTile(TileMap<MapTile> map, int2 position, TileSet tileSet)
	{
		Tile result = null;

		MapTile tile = map[position.x, position.y];

		switch (tile.TileType)
		{
			default:
			case TileType.Space: break;
			case TileType.Floor: result = tileSet.GetTile(0); break;
			case TileType.Wall:
				result = tileSet.GetTile("wall", GetNeighboursMask(map, position, TileType.Wall));
				break;
			case TileType.Door: result = tileSet.GetTile(1); break;
		}

		return result;
	}

	private static byte GetNeighboursMask(TileMap<MapTile> map, int2 position, TileType tileType)
	{
		byte result = 0;
		byte mask = 128; // 10000000

		MapTile neighbour;

		for (int i = 0; i < neighboursCheck.Length; i++)
		{
			neighbour = map[position.x + neighboursCheck[i].x, position.y + neighboursCheck[i].y];

			if (neighbour != null && neighbour.TileType == tileType)
			{
				result += mask;
			}

			mask = (byte)(mask >> 1);
		}

		return result;
	}

	public static uint GetTileId(TileType type)
	{
		uint result = uint.MinValue;

		switch(type)
		{
			case TileType.Space: result = 0; break;
			case TileType.Floor: result = 1; break;
			case TileType.Wall: result = 2; break;
			case TileType.Door: result = 3; break;
		}

		return result;
	}
}
