using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LayoutZone {

    public readonly short id;
    public RectangleInt bounds;

    private Dictionary<Vector2Int, Tile> tiles;

    public List<Vector2Int> ContactArea(LayoutZone other)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        int xMin = 0, xMax = 0, yMin = 0, yMax = 0;
        if (other.bounds.position.y > bounds.y + bounds.height) {               // other is on top 
            yMin = yMax = bounds.y + bounds.height;
            xMin = Mathf.Max(bounds.x, other.bounds.x);
            xMax = Mathf.Min(bounds.x, other.bounds.x);
        } else if (other.bounds.position.x > bounds.x + bounds.width){          // other is right
            xMin = xMax = bounds.x + bounds.width;
            yMin = Mathf.Max(bounds.y, other.bounds.y);
            yMax = Mathf.Min(bounds.y, other.bounds.y);
        } else if (other.bounds.position.y + other.bounds.height < bounds.y) {  // other is down
            yMin = yMax = bounds.y;
            xMin = Mathf.Max(bounds.x, other.bounds.x);
            xMax = Mathf.Min(bounds.x, other.bounds.x);
        } else if (other.bounds.position.x + other.bounds.width < bounds.x) {   // other is left
            xMin = xMax = bounds.x;
            yMin = Mathf.Max(bounds.y, other.bounds.y);
            yMax = Mathf.Min(bounds.y, other.bounds.y);
        }
        for (int i = xMin; i <= xMax; i++) {
            for (int j = yMin; j <= yMax; j++) {
                result.Add(new Vector2Int(i, j));
            }
        }
        return result;
    }

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
