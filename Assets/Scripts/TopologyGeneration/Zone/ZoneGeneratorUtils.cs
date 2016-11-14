public static class ZoneGeneratorUtils
{
	public static int2 FindPlaceForStairs(TileMap<TileType> map, Zone zone)
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

	public static int CountSurroundingFloor(TileMap<TileType> map, Zone zone, int2 tile)
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
}
