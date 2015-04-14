using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : WeightedGraph<Vector2Int> {

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

    private Dictionary<Vector2Int, Tile> tiles;

    public Map()
    {
        tiles = new Dictionary<Vector2Int, Tile>();
    }

    public Tile GetTile(int x, int y)
    {
        Tile result;
        tiles.TryGetValue(new Vector2Int(x, y), out result);
        return result;
    }

    public bool Passable(Vector2Int id)
    {
        return tiles.ContainsKey(id);
    }

    public int Cost(Vector2Int a, Vector2Int b)
    {
        int result;
        Tile bTile;
        if (tiles.TryGetValue(b, out bTile))
        {
            result = bTile.Cost();
        }
        else
        {
            result = int.MaxValue;
        }
        return result;
    }

    public IEnumerable<Vector2Int> Neighbors(Vector2Int id)
    {
        foreach (var dir in DIRS)
        {
            Vector2Int next = new Vector2Int(id.x + dir.x, id.y + dir.y);
            if (Passable(next))
            {
                yield return next;
            }
        }
    }
}
