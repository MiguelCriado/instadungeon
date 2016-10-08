using System.Collections.Generic;

public class TileMap<T>
{
	public int2 Min { get { return min; } }
	public int2 Max { get { return max; } }

	public Layout Layout
	{
		get { return layout; }

		set { layout = value; }
	}

    public int2 spawnPoint;
    public int2 exitPoint;

    private Dictionary<int2, T> tiles;

    private Layout layout;

	private int2 accessor = int2.zero;
	private int2 min = new int2(int.MaxValue, int.MaxValue);
	private int2 max = new int2(int.MinValue, int.MinValue);

	private bool dirty;
	private int2[] cachedPresentTiles;

    public TileMap()
    {
		dirty = true;
        tiles = new Dictionary<int2, T>();
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
				tiles.Add(new int2(accessor.x, accessor.y), value);
				dirty = true;

				if (accessor.x < min.x && accessor.y < min.y)
				{
					min = new int2(accessor.x, accessor.y);
				}

				if (accessor.x > max.x && accessor.y > max.y)
				{
					max = new int2(accessor.x, accessor.y);
				}
			}
		}
	}

	public bool Add(int x, int y, T tile)
	{
		bool result = false;

		if (!tiles.ContainsKey(accessor.Set(x, y)))
		{
			this[x, y] = tile;
			result = true;
		}

		return result;
	}

    public bool Add(int2 position, T tile)
    {
		return Add(position.x, position.y, tile);
    }

    public T GetTile(int x, int y)
    {
		return this[x, y];
    }

	public int2[] GetPresentTiles()
	{
		if (dirty)
		{
			cachedPresentTiles = new int2[tiles.Keys.Count];
			int i = 0;

			foreach (var tile in tiles.Keys)
			{
				cachedPresentTiles[i] = tile;
				i++;
			}

			dirty = false;
		}

		return cachedPresentTiles;
	}

    public Dictionary<int2, T>.Enumerator GetEnumerator()
    {
        return tiles.GetEnumerator();
    }
}
