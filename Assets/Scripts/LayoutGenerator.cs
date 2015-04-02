using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LayoutGenerator : MonoBehaviour {

    [Flags]
    public enum Connections
    {
        None = 0, 
        North = 1, 
        East = 2, 
        South = 4, 
        West = 8
    }

    public static readonly Vector2Int[] DIRS = new[]
        {
            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0), 
        };

    public int height = 4;
    public int width = 4;

    public Vector2 Entrance;
    public Vector2 Exit;

    private Vector2Int mOffsetFrame;
    private int mN;
    private Vector2Int mEntrance;
    private Vector2Int mExit;
    private Connections[,] mLayout;

    public void Start() {
        /*Generate();
        PaintLayout();*/
    }

    public void Update()
    {
        PaintLayout();
    }

    public Connections[,] Generate()
    {
        Initialize();
        GenerateLayout();
        CleanUpLayout();
        Entrance = new Vector2(Hilbert2Layout(mEntrance).x, Hilbert2Layout(mEntrance).y);
        Exit = new Vector2(Hilbert2Layout(mExit).x, Hilbert2Layout(mExit).y);
        return mLayout;
    }

    private void Initialize()
    {
        mLayout = new Connections[width, height];
        InitializeConnections();
        InitializeMembers();
        mEntrance = FindEntrance();
    }

    private void InitializeConnections()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                mLayout[i, j] = Connections.None;
            }
        }
    }

    private void InitializeMembers()
    {
        mN = Math.Max(8, NextPowerOf2(Math.Max(height, width)));
        mOffsetFrame = SetOffset();
    }

    private Vector2Int SetOffset()
    {
        Vector2Int result = new Vector2Int(Random.Range(0, mN - width+1), Random.Range(0, mN - height+1));
        return result;
    }

    private Vector2Int FindEntrance()
    {
        Vector2Int result = null;
        bool found = false;
        int i = 0;
        int max = mN * mN - 1;
        while (!found && i <= max)
        {
            Vector2Int candidate = HilbertCurve.d2xy(mN, i);
            if (IsInsideOffsetFrame(candidate.x, candidate.y)) {
                result = candidate;
                found = true;
            }
            i++;
        }
        return result;
    }

    private void GenerateLayout()
    {
        int initialD = HilbertCurve.xy2d(mN, (int)mEntrance.x, (int)mEntrance.y);
        bool finished = false;
        int i = initialD + 1; 
        int maxSteps = mN * mN - 1;
        int currentPath = 0;
        Connections direction = Connections.None;
        Vector2Int currentTile = new Vector2Int(mEntrance.x, mEntrance.y);
       
        List<List<Vector2Int>> connectedPaths = new List<List<Vector2Int>>();
        connectedPaths.Add(new List<Vector2Int>());
        connectedPaths[currentPath].Add(new Vector2Int(Hilbert2Layout(currentTile).x, Hilbert2Layout(currentTile).y));

        while (!finished && i < maxSteps)
        {
            Vector2Int nextTile = HilbertCurve.d2xy(mN, i);
            if (IsInsideOffsetFrame(nextTile.x, nextTile.y))
            {
                direction = GetDirection(currentTile, nextTile);
                if (direction != Connections.None)
                {
                    Vector2Int layoutTile = new Vector2Int(Hilbert2Layout(currentTile).x, Hilbert2Layout(currentTile).y);
                    mLayout[layoutTile.x, layoutTile.y] |= direction;
                    connectedPaths[currentPath].Add(layoutTile);
                } 
                else 
                {
                    currentPath++;
                    connectedPaths.Add(new List<Vector2Int>());
                }
                currentTile = nextTile;
            }
            i++;
        }
        ConnectUnconnectedPaths(connectedPaths);
        mExit = currentTile;
    }

    private void ConnectUnconnectedPaths(List<List<Vector2Int>> unconnectedPaths)
    {
        int numPaths = unconnectedPaths.Count;
        Debug.Log("numPaths = " + numPaths);
        for (int i = numPaths - 1; i > 0; i--)
        {
            Debug.Log("Connecting path " + i);
            bool connected = false;
            int j = 0;
            while (!connected && j < unconnectedPaths[i].Count)
            {
                Debug.Log("Looking all " + unconnectedPaths[i][j] + "possible matches");
                int k = unconnectedPaths[i - 1].Count - 1;
                while (!connected && k >= 0)
                {
                    Connections direction = GetDirection(unconnectedPaths[i - 1][k], unconnectedPaths[i][j]);
                    Debug.Log("Checking " + unconnectedPaths[i - 1][k] + " & " + unconnectedPaths[i][j]);
                    if (direction != Connections.None)
                    {
                        Debug.Log("Connection found!!!");
                        mLayout[unconnectedPaths[i - 1][k].x, unconnectedPaths[i - 1][k].y] |= direction;
                        connected = true;
                    }
                    k--;
                }
                j++;
            }

        }
    }

    private void CleanUpLayout()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (i > 0 
                    && (mLayout[i-1, j] & Connections.East) == Connections.East)
                {
                    mLayout[i, j] |= Connections.West;
                }
                if (i < width-1
                    && (mLayout[i + 1, j] & Connections.West) == Connections.West)
                {
                    mLayout[i, j] |= Connections.East;
                }
                if (j > 0
                    && (mLayout[i, j - 1] & Connections.North) == Connections.North)
                {
                    mLayout[i, j] |= Connections.South;
                }
                if (j < height-1
                    && (mLayout[i, j + 1] & Connections.South) == Connections.South)
                {
                    mLayout[i, j] |= Connections.North;
                }
            }
        }
    }

    private Connections GetDirection(Vector2Int origin, Vector2Int destiny)
    {
        Connections result = Connections.None;
        if (origin + DIRS[0] == destiny)
        {
            result = Connections.North;
        } else if (origin + DIRS[1] == destiny) 
        {
            result = Connections.East;
        }
        else if (origin + DIRS[2] == destiny)
        {
            result = Connections.South;
        }
        else if (origin + DIRS[3] == destiny)
        {
            result = Connections.West;
        }
        return result;
    }

    private Connections GetSpeculativeDirection(Vector2Int origin, Vector2Int destiny)
    {
        Connections result = Connections.None;
        
        return result;
    }

    private bool IsInsideOffsetFrame(int x, int y)
    {
        bool result = (x >= mOffsetFrame.x 
                       && x < mOffsetFrame.x + width
                       && y >= mOffsetFrame.y 
                       && y < mOffsetFrame.y + height);
        return result;
    }

    private int NextPowerOf2(int x)
    {
        int result = -1;
        if (x >= 0)
        {
            bool found = false;
            int i = 0;
            while (!found)
            {
                if (Mathf.Pow(2, i) > x) {
                    result = (int)Mathf.Pow(2, i);
                    found = true;
                }
                i++;
            }
        }
        return result;
    }

    private Vector2Int Hilbert2Layout(Vector2Int point)
    {
        Vector2Int result = new Vector2Int(point.x - mOffsetFrame.x, point.y - mOffsetFrame.y);
        return result;
    }

    private int ManhattanDistance(Vector2Int origin, Vector2Int destiny)
    {
        int result = Math.Abs((destiny.x - origin.x)) + Math.Abs((destiny.y - origin.y));
        return result;
    }

    private void PaintLayout()
    {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++)
            {
                PaintConnections(mLayout[i, j], i, j);
            }
        }
    }

    private void PaintConnections(Connections cell, int x, int y)
    {
        if ((cell & Connections.North) == Connections.North)
        {
            Debug.DrawLine(new Vector2(x, y), new Vector2(x, y+1));
        }
        if ((cell & Connections.East) == Connections.East)
        {
            Debug.DrawLine(new Vector2(x, y), new Vector2(x+1, y));
        }
        if ((cell & Connections.South) == Connections.South)
        {
            Debug.DrawLine(new Vector2(x, y), new Vector2(x, y-1));
        }
        if ((cell & Connections.West) == Connections.West)
        {
            Debug.DrawLine(new Vector2(x, y), new Vector2(x - 1, y));
        }
    }

}
