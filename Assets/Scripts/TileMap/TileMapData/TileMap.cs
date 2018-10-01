using System.Collections.Generic;
using UnityEngine;

public class TileMap<T>
{
	public Layout Layout { get { return layout; } set { layout = value; } }
	public RectangleInt Bounds { get { return bounds; } }
	public int2 SpawnPoint { get; set; }
	public int2 ExitPoint { get; set; }

	private Dictionary<int2, T> tiles;
	private Layout layout;

	private RectangleInt bounds = new RectangleInt(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);

	private int2 accessor = int2.zero;
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
		get { return this[accessor.Set(x, y)]; }
		set { this[accessor.Set(x, y)] = value; }
	}

	public T this[int2 position]
	{
		get
		{
			if (tiles.ContainsKey(position))
			{
				return tiles[position];
			}
			else
			{
				return default(T);
			}
		}

		set
		{
			if (tiles.ContainsKey(position))
			{
				tiles[position] = value;
			}
			else
			{
				Add(position, value);
			}
		}
	}

	public bool Add(int x, int y, T tile)
	{
		return Add(accessor.Set(x, y), tile);
	}

    public bool Add(int2 position, T tile)
    {
		bool result = false;

		if (!tiles.ContainsKey(position))
		{
			tiles[position] = tile;
			UpdateBounds(position);
			result = true;
			dirty = true;
		}

		return result;
    }

	public void AddSet(Dictionary<int2, T> tileGroup)
	{
		var enumerator = tileGroup.GetEnumerator();

		while (enumerator.MoveNext())
		{
			this[enumerator.Current.Key.x, enumerator.Current.Key.y] = enumerator.Current.Value;
		}
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
			var enumerator = tiles.GetEnumerator();
			int i = 0;

			while (enumerator.MoveNext())
			{
				cachedPresentTiles[i] = enumerator.Current.Key;
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

	protected void UpdateBounds(int2 newTile)
	{
		int xMin = Mathf.Min(newTile.x, bounds.Min.x);
		int xMax = Mathf.Max(newTile.x + 1, bounds.Max.x);
		int yMin = Mathf.Min(newTile.y, bounds.Min.y);
		int yMax = Mathf.Max(newTile.y + 1, bounds.Max.y);

		if (xMin < bounds.Min.x 
			|| xMax > bounds.Max.x
			|| yMin < bounds.Min.y
			|| yMax > bounds.Max.y)
		{
			bounds = new RectangleInt(xMin, yMin, xMax - xMin, yMax - yMin);
		}
	}
}
