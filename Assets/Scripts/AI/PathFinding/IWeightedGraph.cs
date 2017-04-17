using System.Collections.Generic;

public interface IWeightedGraph<L, C>
{
	C Cost(L a, L b);
	IEnumerable<L> Neighbors(L id);
}
