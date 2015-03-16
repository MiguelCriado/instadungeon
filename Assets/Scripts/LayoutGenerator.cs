using UnityEngine;
using System.Collections;
using System;
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

    [Serializable]
    private struct Vector2Int
    {
        public int x;
        public int y;

        public Vector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

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
        mLayout = new Connections[width, height];
        InitializeConnections();
        InitializeMembers();
        mEntrance = FindEntrance();
        GenerateLayout();
        CleanUpLayout();
        Entrance = new Vector2(Hilbert2Layout(mEntrance).x, Hilbert2Layout(mEntrance).y);
        Exit = new Vector2(Hilbert2Layout(mExit).x, Hilbert2Layout(mExit).y);
        return mLayout;
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
        Vector2Int result = new Vector2Int();
        result.x = Random.Range(0, mN - width+1);
        result.y = Random.Range(0, mN - height+1);
        return result;
    }

    private Vector2Int FindEntrance()
    {
        Vector2Int result = new Vector2Int();
        bool found = false;
        int i = 0;
        int x = 0, y = 0;
        int max = mN * mN - 1;
        while (!found && i <= max)
        {
            HilbertCurve.d2xy(mN, i, ref x, ref y);
            if (IsInsideOffsetFrame(x, y)) {
                result.x = x;
                result.y = y;
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
        int i = initialD + 1, maxSteps = mN * mN - 1;
        Vector2Int currentTile = new Vector2Int(mEntrance.x, mEntrance.y);
        Vector2Int nextTile = new Vector2Int();
        while (!finished && i < maxSteps)
        {
            HilbertCurve.d2xy(mN, i, ref nextTile.x, ref nextTile.y);
            //Debug.Log("currentTile = (" + currentTile.x + ", " + currentTile.y + ") \nnextTile = (" + nextTile.x + ", " + nextTile.y + ")");
            if (IsInsideOffsetFrame(nextTile.x, nextTile.y))
            {
                //Debug.Log("Está dentro de offset frame");
                if (GetDirection(currentTile, nextTile) != Connections.None) // if we can connect these two tiles
                {
                    //Debug.Log("podemos conectarlos");
                    Vector2Int layoutTile = new Vector2Int(Hilbert2Layout(currentTile).x, Hilbert2Layout(currentTile).y);
                    //Debug.Log("layoutTile = (" + layoutTile.x + ", " + layoutTile.y + ")");
                    mLayout[layoutTile.x, layoutTile.y] |= GetDirection(currentTile, nextTile);
                    /*if (ManhattanDistance(currentTile, nextTile) == 1)
                    {
                        Vector2Int layoutTile = new Vector2Int(Hilbert2Layout(currentTile).x, Hilbert2Layout(currentTile).y);
                        mLayout[layoutTile.x, layoutTile.y] |= GetDirection(currentTile, nextTile);
                    }
                    else
                    {
                        // TODO hacer esto en condiciones, para cualquier distancia posible. 
                    }*/
                }
                else
                {
                    //Debug.Log("se acabó");
                    finished = true;
                }
                currentTile = nextTile;
            }
            i++;
        }
        //Debug.Log("He hecho " + i + " pasos.");
        mExit = currentTile;
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
        if (origin.x == destiny.x)
        {
            if (origin.y < destiny.y)
            {
                result = Connections.North;
            }
            else if (origin.y > destiny.y)
            {
                result = Connections.South;
            }
        }
        else if (origin.y == destiny.y)
        {
            if (origin.x < destiny.x)
            {
                result = Connections.East;
            }
            else if (origin.x > destiny.x)
            {
                result = Connections.West;
            }
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
