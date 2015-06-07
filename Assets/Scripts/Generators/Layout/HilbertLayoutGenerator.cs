using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HilbertLayoutGenerator : MonoBehaviour, LayoutGenerator {

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

    public int zoneWidth = 15;
    public int zoneHeight = 15;

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

    public Layout Generate()
    {
        Layout result = new Layout();
        Connections[,] layoutArray = GenerateLayoutArray();
        AddZones(result, layoutArray);
        ConnectZones(result, layoutArray);
        result.InitialZone = result.FindZoneByPosition(new Vector2Int((int)Entrance.x, (int)Entrance.y));
        result.FinalZone = result.FindZoneByPosition(new Vector2Int((int)Exit.x, (int)Exit.y));
        return result;
    }

    private void AddZones(Layout layout, Connections[,] layoutArray)
    {
        for (int i = 0; i < layoutArray.GetLength(0); i++)
        {
            for (int j = 0; j < layoutArray.GetLength(1); j++)
            {
                LayoutZone zone = new LayoutZone();
                zone.bounds = new RectangleInt(i * zoneWidth, j * zoneHeight, zoneWidth, zoneHeight);
                layout.Add(zone);
            }
        }
    }

    private void ConnectZones(Layout result, Connections[,] layoutArray)
    {
        for (int i = 0; i < layoutArray.GetLength(0); i++)
        {
            for (int j = 0; j < layoutArray.GetLength(1); j++)
            {
                LayoutZone currentZone = result.FindZoneByPosition(new Vector2Int(i * zoneWidth, j * zoneHeight));
                if (i > 0
                    && (layoutArray[i, j] & Connections.East) == Connections.East)
                {
                    result.ConnectZones(currentZone, result.FindZoneByPosition(new Vector2Int((i + 1) * zoneWidth, j * zoneHeight)));
                }
                if (i < width - 1
                    && (layoutArray[i, j] & Connections.West) == Connections.West)
                {
                    result.ConnectZones(currentZone, result.FindZoneByPosition(new Vector2Int((i - 1) * zoneWidth, j * zoneHeight)));
                }
                if (j > 0
                    && (layoutArray[i, j] & Connections.North) == Connections.North)
                {
                    result.ConnectZones(currentZone, result.FindZoneByPosition(new Vector2Int(i * zoneWidth, (j + 1) * zoneHeight)));
                }
                if (j < height - 1
                    && (layoutArray[i, j] & Connections.South) == Connections.South)
                {
                    result.ConnectZones(currentZone, result.FindZoneByPosition(new Vector2Int(i * zoneWidth, (j - 1) * zoneHeight)));
                }
            }
        }
    }

    public Connections[,] GenerateLayoutArray()
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
        int i = initialD; 
        int maxSteps = mN * mN - 1;
        Connections direction = Connections.None;
        List<List<Vector2Int>> unconnectedPaths = new List<List<Vector2Int>>();
        List<Vector2Int> currentPath = new List<Vector2Int>();
        Vector2Int currentTile = new Vector2Int(mEntrance.x, mEntrance.y);
        currentPath.Add(Hilbert2Layout(currentTile));

        while (i < maxSteps)
        {
            Vector2Int nextTile = HilbertCurve.d2xy(mN, i + 1);
            if (IsInsideOffsetFrame(nextTile.x, nextTile.y))
            {
                direction = GetDirection(currentTile, nextTile);
                if (direction != Connections.None)
                {
                    Vector2Int layoutTile = Hilbert2Layout(currentTile);
                    mLayout[layoutTile.x, layoutTile.y] |= direction; 
                }
                else
                {
                    unconnectedPaths.Add(currentPath);
                    currentPath = new List<Vector2Int>();
                }
                currentPath.Add(Hilbert2Layout(nextTile));
                currentTile = nextTile;
            }
            
            i++;
        }
        unconnectedPaths.Add(currentPath);
        ConnectUnconnectedPaths(unconnectedPaths);
        mExit = currentTile;
    }

    private void ConnectUnconnectedPaths(List<List<Vector2Int>> unconnectedPaths)
    {
        int numPaths = unconnectedPaths.Count;
        for (int i = numPaths - 1; i > 0; i--)
        {
            bool connected = false;
            int j = 0;
            while (!connected && j < unconnectedPaths[i].Count)
            {
                int k = unconnectedPaths[i - 1].Count - 1;
                while (!connected && k >= 0)
                {
                    Connections direction = GetDirection(unconnectedPaths[i - 1][k], unconnectedPaths[i][j]);
                    if (direction != Connections.None)
                    {
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
