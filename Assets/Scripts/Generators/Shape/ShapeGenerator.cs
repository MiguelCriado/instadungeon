using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ShapeGenerator {

    Dictionary<Vector2Int, Tile> Generate(int width, int height, Vector2Int offset);
    Shape.ConnectionTime GetConnectionTime();
    void WipeEntrances();
    void SetEntrance(Vector2Int point);
}
