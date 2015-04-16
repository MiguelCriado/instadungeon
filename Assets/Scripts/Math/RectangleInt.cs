using UnityEngine;
using System.Collections;

public class RectangleInt : MonoBehaviour {

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
}
