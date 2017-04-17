using System;
using System.Collections.Generic;

public class AStarSearch<L, C> where C : IComparable<C>
{
	private IWeightedGraph<L, C> graph;
	private IAStarHeuristic<L, C> defaultHeuristic;
	private PriorityQueue<L, C> frontier;
	private Dictionary<L, L> cameFrom;
	private Dictionary<L, C> costSoFar;

	// Note: a generic version of A* would abstract over int2 and also Heuristic
	// public static U Heuristic(T a, T b)
	// {
		// return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
	// }

	public AStarSearch(IWeightedGraph<L, C> graph, IAStarHeuristic<L, C> heuristic)
	{
		this.graph = graph;
		defaultHeuristic = heuristic;
		frontier = new PriorityQueue<L, C>();
		cameFrom = new Dictionary<L, L>();
		costSoFar = new Dictionary<L, C>();
	}

	public L[] Search(L start, L goal)
	{
		return Search(start, goal, defaultHeuristic);
	}

	public L[] Search(L start, L goal, IAStarHeuristic<L, C> heuristic)
	{
		L[] result;
		frontier.Clear();
		cameFrom.Clear();
		costSoFar.Clear();

		frontier.Enqueue(start, default(C));
		cameFrom.Add(start, start);
		costSoFar.Add(start, default(C));

		while (frontier.Count() > 0)
		{
			var current = frontier.Dequeue();

			if (current.Equals(goal))
			{
				break;
			}

			foreach (var next in graph.Neighbors(current))
			{
				C newCost = heuristic.Sum(costSoFar[current], graph.Cost(current, next));

				if (!costSoFar.ContainsKey(next) || newCost.CompareTo(costSoFar[next]) < 0)
				{
					AddOrUpdate(costSoFar, next, newCost);
					C priority = heuristic.Sum(newCost, heuristic.Evaluate(next, goal));
					frontier.Enqueue(next, priority);
					AddOrUpdate(cameFrom, next, current);
				}
			}
		}

		result = new L[cameFrom.Count];
		L currentLocation = goal;

		for (int i = result.Length - 1; i >= 0; i--)
		{
			result[i] = currentLocation;
			cameFrom.TryGetValue(currentLocation, out currentLocation);
		}

		return result;
	}

	private void AddOrUpdate<T, U>(Dictionary<T, U> dict, T key, U value)
	{
		if (dict.ContainsKey(key))
		{
			dict[key] = value;
		}
		else
		{
			dict.Add(key, value);
		}
	}
}
