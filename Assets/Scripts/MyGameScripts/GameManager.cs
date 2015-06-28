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
        Tile result = map.Map.GetTile(x, y);
        return result;
    }

    /*public List<Vector2Int> FindPath(Vector2 start, Vector2 goal)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        Vector2Int startLocation = new Vector2Int((int)start.x, (int)start.y);
        Vector2Int goalLocation = new Vector2Int((int)goal.x, (int)goal.y);
        //AStarSearch astar = new AStarSearch(map.PathFindingMap, startLocation, goalLocation);
        Vector2Int currentLocation = goalLocation;
        bool noPath = false;
        result.Add(currentLocation);
        while (!noPath && !currentLocation.Equals(startLocation))
        {
            Vector2Int previousLocation;
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
    }*/
}
