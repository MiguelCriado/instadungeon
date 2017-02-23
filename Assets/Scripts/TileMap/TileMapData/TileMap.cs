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

	public T this[int2 position]
	{
		get { return this[position.x, position.y]; }
		set { this[position.x, position.y] = value; }
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
				UpdateBounds(accessor);
				dirty = true;
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
