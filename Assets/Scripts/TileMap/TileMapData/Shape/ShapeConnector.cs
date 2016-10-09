using System.Collections.Generic;
using Random = UnityEngine.Random;

public static class ShapeConnector
{
    public static TileMap<TileType> BuildMap(LayoutGenerator layoutG, ShapeGenerator shapeG, int levelSeed)
    {
        Random.InitState(levelSeed);
        TileMap<TileType> result = new TileMap<TileType>();
        Layout layout = layoutG.Generate();
		result.Layout = layout;

        if (shapeG.GetConnectionTime() == Shape.ConnectionTime.PreConnection)
        {
            // Creating the connections
            List<LayoutZone> zonesToConnect = new List<LayoutZone>();

            if (layout.Zones.Count > 0)
			{
                zonesToConnect.Add(layout.InitialZone);

                while (zonesToConnect.Count > 0)
                {
                    LayoutZone zone = zonesToConnect[0];
                    zonesToConnect.Remove(zone);

                    foreach (LayoutZone neighbour in layout.GetAdjacentZones(zone))
                    {
                        if (!zone.connections.ContainsValue(neighbour))
                        {
							int2 connectionPoint;
                            List<int2> connectionCandidates = zone.bounds.ContactArea(neighbour.bounds, true);
                            connectionPoint = connectionCandidates[Random.Range(0, connectionCandidates.Count - 1)];
                            zone.AddConnectionPoint(connectionPoint, neighbour);

							int2 contactPoint;
							
							if (neighbour.ContactPoint(connectionPoint, out contactPoint))
							{
								neighbour.AddConnectionPoint(contactPoint, zone);
								zonesToConnect.Add(neighbour);
							}
                        }
                    }
                }
            }

            // Generating shapes

            ICollection<LayoutZone> zones = layout.Zones;

            foreach (LayoutZone currentZone in zones)
            {
                Dictionary<int2, TileType> shape;
                shapeG.WipeEntrances();

                foreach (KeyValuePair<int2, LayoutZone> connection in currentZone.connections)
				{
                    shapeG.SetEntrance(currentZone.Map2Zone(connection.Key));
                }

                shape = shapeG.Generate(currentZone.bounds.width, currentZone.bounds.height, currentZone.bounds.position);

                foreach (KeyValuePair<int2, TileType> tile in shape)
                {
                    currentZone.tiles.Add(tile.Key);
                    result.Add(tile.Key, tile.Value);
                }
            }
        }
        else
        {

        }

        // Placing Stairs
        LayoutZone initialZone = result.Layout.InitialZone;
        FindPlaceForStairs(result, initialZone, out result.spawnPoint);

        return result;
    }

    private static bool FindPlaceForStairs(TileMap<TileType> map, LayoutZone zone, out int2 stairsLocation)
    {
        bool result = false;
		stairsLocation = int2.zero;
        int currentSurroundingFloorTiles = -1;
        int targetSurroundingFloorTiles = 8;

        foreach (int2 tile in zone.tiles)
        {
            int thisTileSurroundingFloorTiles = CountSurroundingFloor(map, zone, tile);

            if (map.GetTile(tile.x, tile.y) == TileType.Floor && thisTileSurroundingFloorTiles >= currentSurroundingFloorTiles)
            {
                stairsLocation = tile;
                currentSurroundingFloorTiles = thisTileSurroundingFloorTiles;
				result = true;

                if (currentSurroundingFloorTiles >= targetSurroundingFloorTiles)
                {
                    break;
                }
            }
        }

        return result;
    }

    private static int CountSurroundingFloor(TileMap<TileType> map, LayoutZone zone, int2 tile)
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
