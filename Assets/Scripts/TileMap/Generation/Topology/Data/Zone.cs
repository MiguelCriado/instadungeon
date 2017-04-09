using System.Collections;
using System.Collections.Generic;

public class Zone : IEnumerable
{
    private static int id_counter = 0;

	public Layout ParentLayout { get; set; }

    public readonly int id;
    public RectangleInt bounds;
    public HashSet<int2> tiles;
    public Dictionary<int2, Zone> connections;

    public Zone()
    {
        id = id_counter++;
        Init(0, 0, 0, 0);
    }

    public Zone(int x, int y, int width, int height)
    {
        id = id_counter++;
        Init(x, y, width, height);
    }

    private void Init(int x, int y, int width, int height)
    {
        connections = new Dictionary<int2, Zone>();
        tiles = new HashSet<int2>();
        bounds = new RectangleInt(x, y, width, height);
    }

    public void AddConnectionPoint(int2 point, Zone layoutZone)
    {
        connections.Add(point, layoutZone);
    }

    public IEnumerator<int2> GetEnumerator()
    {
        return tiles.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    } 

    /// <summary>
    /// Finds the tile inside this LayoutZone that is adjacent to the provided point.  
    /// </summary>
    /// <param name="layoutZonePoint"></param>
    /// <returns></returns>
    public bool ContactPoint(int2 layoutZonePoint, out int2 contactPoint)
	{
        return bounds.ContactPoint(layoutZonePoint, out contactPoint);
    }

    public int2 MapToZone(int2 mapPosition)
    {
        return mapPosition - bounds.Position;
    }

    public int2 ZoneToMap(int2 zonePosition)
    {
        return bounds.Position + zonePosition;
    }

    public override string ToString()
    {
        return id.ToString();
    }
}
