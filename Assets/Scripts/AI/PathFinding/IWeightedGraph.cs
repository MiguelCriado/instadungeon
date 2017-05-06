using System.Collections.Generic;

/// <summary>
/// Interface of a WeightedGraph to be used with AStarSearch
/// </summary>
/// <typeparam name="L">Location type</typeparam>
/// <typeparam name="C">Cost type</typeparam>
public interface IWeightedGraph<L, C>
{
	C Cost(L a, L b);
	IEnumerable<L> Neighbors(L id);
}
