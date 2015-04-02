using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AStarSearch
{
    public Dictionary<Vector2Int, Vector2Int> cameFrom
        = new Dictionary<Vector2Int, Vector2Int>();
    public Dictionary<Vector2Int, int> costSoFar
        = new Dictionary<Vector2Int, int>();

    // Note: a generic version of A* would abstract over Location and
    // also Heuristic
    static public int Heuristic(Vector2Int a, Vector2Int b)
    {
        return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
    }

    public AStarSearch(WeightedGraph<Vector2Int> graph, Vector2Int start, Vector2Int goal)
    {
        var frontier = new PriorityQueue<Vector2Int, int>();
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

    public void AddOrUpdate(Dictionary<Vector2Int, int> dict, Vector2Int key, int value)
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

    public void AddOrUpdate(Dictionary<Vector2Int, Vector2Int> dict, Vector2Int key, Vector2Int value)
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
