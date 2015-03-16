using System.Collections.Generic;

public interface WeightedGraph<L>
{
    int Cost(Location a, Location b);
    IEnumerable<Location> Neighbors(Location id);
}
