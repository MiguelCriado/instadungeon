using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LayoutZone {

    public readonly short id;
    public RectangleInt bounds;

    private Dictionary<Vector2Int, Tile> tiles;

    public Vector2Int ContactPoint(Vector2Int layoutZonePoint) {
        return new Vector2Int(0,0);
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
