using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AStarSearch
{
    public Dictionary<Location, Location> cameFrom
        = new Dictionary<Location, Location>();
    public Dictionary<Location, int> costSoFar
        = new Dictionary<Location, int>();

    // Note: a generic version of A* would abstract over Location and
    // also Heuristic
    static public int Heuristic(Location a, Location b)
    {
        return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
    }

    public AStarSearch(WeightedGraph<Location> graph, Location start, Location goal)
    {
        var frontier = new PriorityQueue<Location, int>();
        frontier.Enqueue(start, 0);

        cameFrom.Add(start, start);
        costSoFar.Add(start, 0);

        while (frontier.Count() > 0)
        {
            var current = frontier.Dequeue();

            if (current.Equals(goal))
            {
                break;
            }
            
            foreach (var next in graph.Neighbors(current))
            {
                int newCost = costSoFar[current] + graph.Cost(current, next);
                if (!costSoFar.ContainsKey(next)
                    || newCost < costSoFar[next])
                {
                    AddOrUpdate(costSoFar, next, newCost);
                    int priority = newCost + Heuristic(next, goal);
                    frontier.Enqueue(next, priority);
                    AddOrUpdate(cameFrom, next, current);
                }
            }
        }
    }

    public void AddOrUpdate(Dictionary<Location, int> dict, Location key, int value)
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

    public void AddOrUpdate(Dictionary<Location, Location> dict, Location key, Location value)
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
