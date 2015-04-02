using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SquareGrid : WeightedGraph<Vector2Int>
{
    // Implementation notes: I made the fields public for convenience,
    // but in a real project you'll probably want to follow standard
    // style and make them private.

    public static readonly Vector2Int[] DIRS = new[]
        {
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1), 
            /*new Location(1, 1),
            new Location(-1, 1),
            new Location(-1, -1),
            new Location(1, -1)*/
 
        };

    public int width, height;
    public HashSet<Vector2Int> floorTiles = new HashSet<Vector2Int>();

    public SquareGrid(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public bool InBounds(Vector2Int id)
    {
        return 0 <= id.x && id.x < width
            && 0 <= id.y && id.y < height;
    }

    public bool Passable(Vector2Int id)
    {
        return floorTiles.Contains(id);
    }

    public int Cost(Vector2Int a, Vector2Int b)
    {
        return GameManager.Instance.GetTile(b.x, b.y).Cost();
    }

    public IEnumerable<Vector2Int> Neighbors(Vector2Int id)
    {
        foreach (var dir in DIRS)
        {
            Vector2Int next = new Vector2Int(id.x + dir.x, id.y + dir.y);
            if (InBounds(next) && Passable(next))
            {
                yield return next;
            }
        }
    }
}
