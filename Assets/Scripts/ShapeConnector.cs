using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

public static class ShapeConnector {

    public static Map<BlueprintAsset> BuildMap(LayoutGenerator layoutG, ShapeGenerator shapeG)
    {
        return BuildMap(layoutG, shapeG, Random.seed);
    }

    public static Map<BlueprintAsset> BuildMap(LayoutGenerator layoutG, ShapeGenerator shapeG, int levelSeed)
    {
        Random.seed = levelSeed;
        Map<BlueprintAsset> result = new Map<BlueprintAsset>();
        Layout layout = layoutG.Generate();
        result.SetLayout(layout);

        if (shapeG.GetConnectionTime() == Shape.ConnectionTime.PreConnection)
        {
            // Creating the connections
            List<LayoutZone> zonesToConnect = new List<LayoutZone>();
            if (layout.Zones.Count > 0) {
                zonesToConnect.Add(layout.InitialZone);
                while (zonesToConnect.Count > 0)
                {
                    LayoutZone zone = zonesToConnect[0];
                    zonesToConnect.Remove(zone);
                    foreach (LayoutZone neighbour in layout.GetAdjacentZones(zone))
                    {
                        if (!zone.connections.ContainsValue(neighbour))
                        {
                            Vector2Int connectionPoint;
                            List<Vector2Int> connectionCandidates = zone.bounds.ContactArea(neighbour.bounds, true);
                            connectionPoint = connectionCandidates[Random.Range(0, connectionCandidates.Count - 1)];
                            zone.AddConnectionPoint(connectionPoint, neighbour);
                            neighbour.AddConnectionPoint(neighbour.ContactPoint(connectionPoint), zone);
                            zonesToConnect.Add(neighbour);
                        }
                    }
                }
            }

            // Generating shapes

            ICollection<LayoutZone> zones = layout.Zones;
            foreach (LayoutZone currentZone in zones)
            {
                Dictionary<Vector2Int, BlueprintAsset> shape;
                shapeG.WipeEntrances();
                foreach (KeyValuePair<Vector2Int, LayoutZone> connection in currentZone.connections) {
                    shapeG.SetEntrance(currentZone.Map2Zone(connection.Key));
                }
                shape = shapeG.Generate(currentZone.bounds.width, currentZone.bounds.height, currentZone.bounds.position);
                foreach (KeyValuePair<Vector2Int, BlueprintAsset> tile in shape)
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
        LayoutZone initialZone = result.GetLayout().InitialZone;
        result.spawnPoint = FindPlaceForStairs(result, initialZone);
        return result;
    }

    private static Vector2Int FindPlaceForStairs(Map<BlueprintAsset> map, LayoutZone zone)
    {
        Vector2Int result = null;
        int currentSurroundingFloorTiles = -1;
        int targetSurroundingFloorTiles = 8;
        foreach (Vector2Int tile in zone.tiles)
        {
            int thisTileSurroundingFloorTiles = CountSurroundingFloor(map, zone, tile);
            if (map.GetTile(tile.x, tile.y) == BlueprintAsset.Floor && thisTileSurroundingFloorTiles >= currentSurroundingFloorTiles)
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

    private static int CountSurroundingFloor(Map<BlueprintAsset> map, LayoutZone zone, Vector2Int tile)
    {
        int result = 0;
        Vector2Int[] dirs = new[]
        {
            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0), 
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 1),
            new Vector2Int(1, 1),
            new Vector2Int(1, -1), 
        };
        for (int i = 0; i < dirs.Length; i++)
        {
            Vector2Int adjacentTile = tile + dirs[i];
            if (zone.tiles.Contains(tile + dirs[i]) && map.GetTile(adjacentTile.x, adjacentTile.y) == BlueprintAsset.Floor)
            {
                result++;
            }
        }
        return result;
    }
}
