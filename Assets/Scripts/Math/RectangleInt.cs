using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RectangleInt {

    public Vector2Int position
    {
        get { return new Vector2Int(x, y); }
    }

    public int x;
    public int y;
    public int width;
    public int height;

    public RectangleInt(int x, int y, int width, int height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }

    public static bool operator ==(RectangleInt a, RectangleInt b)
    {
        return a.x == b.x
            && a.y == b.y
            && a.width == b.width
            && a.height == b.height;
    }

    public static bool operator !=(RectangleInt a, RectangleInt b)
    {
        return !(a == b);
    }

    public bool Contains(Vector2Int point)
    {
        return point.x >= x 
            && point.x < point.x + width
            && point.y >= y 
            && point.y < point.y + height;
    }

    public bool Overlaps(RectangleInt other)
    {
        return x < other.x + other.width
            && x + width > other.x
            && y < other.y + other.height
            && y + height > other.y;
    }

    public bool IsAdjacent(RectangleInt other)
    {
        return ((y + height == other.y || y == other.y + other.height) && (x < other.x + other.width || x + width > other.x))
            || ((x + width == other.x || x == other.x + other.width) && (y < other.y + other.height || y + height > other.y));
    }

    public List<Vector2Int> ContactArea(RectangleInt other)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        int xMin = 0, xMax = 0, yMin = 0, yMax = 0;
        if (IsAdjacent(other))
        {
            if (other.y >= y + height)
            { // other is on top 
                yMin = yMax = y + height - 1;
                xMin = Mathf.Max(x, other.x);
                xMax = Mathf.Min(x + width - 1, other.x + other.width - 1);
            }
            else if (other.x >= x + width)
            { // other is right
                xMin = xMax = x + width - 1;
                yMin = Mathf.Max(y, other.y);
                yMax = Mathf.Min(y + height - 1, other.y + other.height - 1);
            }
            else if (other.y + other.height <= y)
            { // other is down
                yMin = yMax = y;
                xMin = Mathf.Max(x, other.x);
                xMax = Mathf.Min(x + width - 1, other.x + other.width - 1);
            }
            else if (other.x + other.width <= x)
            { // other is left
                xMin = xMax = x;
                yMin = Mathf.Max(y, other.y);
                yMax = Mathf.Min(y + height - 1, other.y + other.height - 1);
            }
            for (int i = xMin; i <= xMax; i++)
            {
                for (int j = yMin; j <= yMax; j++)
                {
                    result.Add(new Vector2Int(i, j));
                }
            }
        }
        
        return result;
    }
}
