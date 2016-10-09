using System.Collections.Generic;

public interface ShapeGenerator
{
    Dictionary<int2, TileType> Generate(int width, int height, int2 offset);
    Shape.ConnectionTime GetConnectionTime();
    void WipeEntrances();
    void SetEntrance(int2 point);
}
