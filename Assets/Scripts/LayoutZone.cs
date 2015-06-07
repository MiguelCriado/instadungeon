using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LayoutZone {

    private static int id_counter = 0;

    public readonly int id;
    public RectangleInt bounds;
   
    public Dictionary<Vector2Int, Tile> tiles;

    public Dictionary<Vector2Int, LayoutZone> connections;
    

    public LayoutZone()
    {
        this.id = id_counter++;
        Init(0, 0, 0, 0);
    }

    public LayoutZone(int x, int y, int width, int height)
    {
        this.id = id_counter++;
        Init(x, y, width, height);
    }

    private void Init(int x, int y, int width, int height)
    {
        connections = new Dictionary<Vector2Int, LayoutZone>();
        tiles = new Dictionary<Vector2Int, Tile>();
        bounds = new RectangleInt(x, y, width, height);
    }

    public void AddConnectionPoint(Vector2Int point, LayoutZone layoutZone)
    {
        connections.Add(point, layoutZone);
    }

    /// <summary>
    /// Finds the tile inside this LayoutZone that is adjacent to the provided point.  
    /// </summary>
    /// <param name="layoutZonePoint"></param>
    /// <returns></returns>
    public Vector2Int ContactPoint(Vector2Int layoutZonePoint) {
        return bounds.ContactPoint(layoutZonePoint);
    }

    public Vector2Int Map2Zone(Vector2Int mapPosition)
    {
        return bounds.position - mapPosition;
    }

    public Vector2Int Zone2Map(Vector2Int zonePosition)
    {
        return bounds.position + zonePosition;
    }

    public override string ToString()
    {
        return id.ToString();
    }
}
