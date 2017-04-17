using System.Collections.Generic;

public class SquareGrid : IWeightedGraph<int2, int>
{
    // Implementation notes: I made the fields public for convenience,
    // but in a real project you'll probably want to follow standard
    // style and make them private.

    private static readonly int2[] DIRS = new[]
        {
            new int2(1, 0),
            new int2(0, -1),
            new int2(-1, 0),
            new int2(0, 1), 
            /*new Location(1, 1),
            new Location(-1, 1),
            new Location(-1, -1),
            new Location(1, -1)*/
 
        };

    private int width, height;
    private HashSet<int2> floorTiles = new HashSet<int2>();

    public SquareGrid(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public bool InBounds(int2 id)
    {
        return 
			0 <= id.x
			&& id.x < width
			&& 0 <= id.y
			&& id.y < height;
    }

    public bool Passable(int2 id)
    {
        return floorTiles.Contains(id);
    }

    public int Cost(int2 a, int2 b)
    {
		// TODO: calculate cost
		return 0;
    }

    public IEnumerable<int2> Neighbors(int2 id)
    {
        foreach (var dir in DIRS)
        {
			int2 next = new int2(id.x + dir.x, id.y + dir.y);

            if (InBounds(next) && Passable(next))
            {
                yield return next;
            }
        }
    }
}
