using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LayoutZone {

    public readonly short id;
    public RectangleInt bounds;
    public List<Vector2Int> Exits
    {
        get
        {
            return connections;
        }
    }
    public Dictionary<Vector2Int, Tile> tiles;

    private List<Vector2Int> connections;
    

    public LayoutZone()
    {
        connections = new List<Vector2Int>();
        tiles = new Dictionary<Vector2Int, Tile>();
    }

    public void AddConnectionPoint(Vector2Int point)
    {
        connections.Add(point);
    }

    /// <summary>
    /// Finds the tile inside this LayoutZone that is adjacent to the provided point.  
    /// </summary>
    /// <param name="layoutZonePoint"></param>
    /// <returns></returns>
    public Vector2Int ContactPoint(Vector2Int layoutZonePoint) {
        return bounds.ContactPoint(layoutZonePoint);
    }

    public Vector2Int Map2Zone(Vector2Int mapPosition)
    {
        return bounds.position - mapPosition;
    }

    public Vector2Int Zone2Map(Vector2Int zonePosition)
    {
        return bounds.position + zonePosition;
    }
}
