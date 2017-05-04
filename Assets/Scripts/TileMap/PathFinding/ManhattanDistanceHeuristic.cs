using UnityEngine;

public class ManhattanDistanceHeuristic : IAStarHeuristic<int2, int>
{
	public int Evaluate(int2 a, int2 b)
	{
		return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
	}

	public int Sum(int a, int b)
	{
		return a + b;
	}
}
