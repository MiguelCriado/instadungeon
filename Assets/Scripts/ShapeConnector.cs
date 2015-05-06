using System.Collections.Generic;
using Random = UnityEngine.Random;

public static class ShapeConnector {

    public static Map BuildMap(LayoutGenerator layoutG, ShapeGenerator shapeG)
    {
        Map result = new Map();
        Layout layout = layoutG.Generate();

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
                    foreach (Node<LayoutZone> neighbour in layout.GetAdjacentZones(zone))
                    {
                        Vector2Int connectionPoint;
                        List<Vector2Int> connectionCandidates = zone.bounds.ContactArea(neighbour.Value.bounds);
                        connectionCandidates.RemoveAll(point => 
                                point.x == zone.bounds.x ||
                                point.x == (zone.bounds.x + zone.bounds.width - 1) ||
                                point.y == zone.bounds.y ||
                                point.y == (zone.bounds.y + zone.bounds.height - 1)
                            );
                        connectionPoint = connectionCandidates[Random.Range(0, connectionCandidates.Count)];
                        zone.AddConnectionPoint(connectionPoint);
                        neighbour.Value.AddConnectionPoint(neighbour.Value.ContactPoint(connectionPoint));
                        zonesToConnect.Add(neighbour.Value);
                    }
                }
            }

            // Generating shapes

            for (int i = 0; i < layout.Zones.Count; i++)
            {
                Dictionary<Vector2Int, Tile> shape;
                LayoutZone zone = layout.Zones[i].Value;
                shapeG.WipeEntrances();
                for (int j = 0; j < zone.Exits.Count; j++)
                {
                    shapeG.SetEntrance(zone.Exits[j]);
                }
                shape = shapeG.Generate(zone.bounds.width, zone.bounds.height, zone.bounds.position);
                foreach (KeyValuePair<Vector2Int, Tile> tile in shape)
                {
                    zone.tiles.Add(tile.Key, tile.Value);
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
