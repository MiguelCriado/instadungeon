using UnityEngine;

public class Tile
{
	public uint Id { get; private set; }
	public Rect[] Rects { get; private set; }
	public Rect[] UvRects { get; private set; }

	public Tile(uint id, Rect[] rects, Rect[] uvRects)
	{
		Id = id;
		Rects = rects;
		UvRects = uvRects;
	}
}
