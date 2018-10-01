using System;
using System.Collections.Generic;

/// <summary>
/// An A* generic pathfinding class. 
/// </summary>
/// <typeparam name="L">Location type</typeparam>
/// <typeparam name="C">Cost type</typeparam>
public class AStarSearch<L, C> where C : IComparable<C>
{
	private IWeightedGraph<L, C> defaultGraph;
	private IAStarHeuristic<L, C> defaultHeuristic;
	private PriorityQueue<L, C> frontier;
	private Dictionary<L, L> cameFrom;
	private Dictionary<L, C> costSoFar;

	public AStarSearch(IWeightedGraph<L, C> graph, IAStarHeuristic<L, C> heuristic)
	{
		defaultGraph = graph;
		defaultHeuristic = heuristic;
		frontier = new PriorityQueue<L, C>();
		cameFrom = new Dictionary<L, L>();
		costSoFar = new Dictionary<L, C>();
	}

	public L[] Search(L start, L goal)
	{
		return Search(start, goal, defaultGraph, defaultHeuristic);
	}

	public L[] Search(L start, L goal, IWeightedGraph<L, C> graph)
	{
		return Search(start, goal, graph, defaultHeuristic);
	}

	public L[] Search(L start, L goal, IWeightedGraph<L, C> graph, IAStarHeuristic<L, C> heuristic)
	{
		List<L> result;
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

			var enumerator = graph.Neighbors(current).GetEnumerator();
			L next;

			while (enumerator.MoveNext())
			{
				next = enumerator.Current;

				C newCost = heuristic.Sum(costSoFar[current], graph.Cost(current, next));

				if (!costSoFar.ContainsKey(next) || newCost.CompareTo(costSoFar[next]) < 0)
				{
					costSoFar[next] = newCost;
					C priority = heuristic.Sum(newCost, heuristic.Evaluate(next, goal));
					frontier.Enqueue(next, priority);
					cameFrom[next] = current;
				}
			}
		}

		result = new List<L>();
		L currentLocation = goal;
		bool pathNotFound = false;

		while (!pathNotFound && !currentLocation.Equals(start))
		{
			result.Insert(0, currentLocation);

			if (!cameFrom.TryGetValue(currentLocation, out currentLocation))
			{
				pathNotFound = true;
				result.Clear();
			}
		}

		return result.ToArray();
	}
}
