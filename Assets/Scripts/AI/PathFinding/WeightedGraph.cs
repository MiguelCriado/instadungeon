using System.Collections.Generic;

public interface WeightedGraph<L>
{
    int Cost(Vector2Int a, Vector2Int b);
    IEnumerable<Vector2Int> Neighbors(Vector2Int id);
}
