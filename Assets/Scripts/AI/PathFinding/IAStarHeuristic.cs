using System;

public interface IAStarHeuristic<L, C> where C : IComparable<C>
{
	C Evaluate(L a, L b);
	C Sum(C a, C b);
}
