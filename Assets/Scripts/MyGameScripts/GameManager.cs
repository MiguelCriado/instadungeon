using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager> {

    public enum Phase
    {
        Idle,
        Move,
        Attack
    }

    public Vector2 characterPosition;

    Phase CurrentPhase;
    private MapHandler map;

	// Use this for initialization
	void Start () {
	    map = FindObjectOfType<MapHandler>() as MapHandler;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public Tile GetTile(int x, int y)
    {
        Tile result = null;
        if (0 <= x && x < map.Map.GetLength(0)
            && 0 <= y && y < map.Map.GetLength(1))
        {
            result = map.Map[x, y];
        }
        return result;
    }

    public List<Location> FindPath(Vector2 start, Vector2 goal)
    {
        List<Location> result = new List<Location>();
        Location startLocation = new Location((int)start.x, (int)start.y);
        Location goalLocation = new Location((int)goal.x, (int)goal.y);
        AStarSearch astar = new AStarSearch(map.PathFindingMap, startLocation, goalLocation);
        Location currentLocation = goalLocation;
        bool noPath = false;
        result.Add(currentLocation);
        while (!noPath && !currentLocation.Equals(startLocation))
        {
            Location previousLocation;
            astar.cameFrom.TryGetValue(currentLocation, out previousLocation);
            if (previousLocation != null) {
                result.Insert(0, previousLocation);
                currentLocation = previousLocation;
            }
            else
            {
                noPath = true;
            }
        }

        if (noPath)
        {
            Debug.Log("Path not found :(");
            result = null;
        }
        return result;
    }
}
