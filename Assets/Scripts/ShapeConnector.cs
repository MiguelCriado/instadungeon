using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

public static class ShapeConnector {

    public static Map<BlueprintAsset> BuildMap(LayoutGenerator layoutG, ShapeGenerator shapeG)
    {
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
        return result;
    }
}
