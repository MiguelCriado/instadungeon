using System.Collections.Generic;

public class Map<T>
{
	public Layout Layout
	{
		get { return layout; }

		set { layout = value; }
	}

    public Vector2Int spawnPoint;
    public Vector2Int exitPoint;

    private Dictionary<Vector2Int, T> tiles;
    private Layout layout;

	private Vector2Int accessor = new Vector2Int(0, 0);

    public Map()
    {
        tiles = new Dictionary<Vector2Int, T>();
        layout = new Layout();
    }

	public T this[int x, int y]
	{
		get
		{
			if (tiles.ContainsKey(accessor.Set(x, y)))
			{
				return tiles[accessor];
			}
			else
			{
				return default(T);
			}
		}

		set
		{
			if (tiles.ContainsKey(accessor.Set(x, y)))
			{
				tiles[accessor] = value;
			}
			else
			{
				tiles.Add(accessor, value);
			}
		}
	}

    public void Add(Vector2Int position, T tile)
    {
        tiles.Add(position, tile);
    }

    public T GetTile(int x, int y)
    {
        T result;
        tiles.TryGetValue(new Vector2Int(x, y), out result);
        return result;
    }

    public Dictionary<Vector2Int, T>.Enumerator GetEnumerator()
    {
        return tiles.GetEnumerator();
    }
    /*
    public bool Passable(Vector2Int id)
    {
        return tiles.ContainsKey(id);
    }

    public int Cost(Vector2Int a, Vector2Int b)
    {
        int result;
        T bTile;
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
    }*/
}
