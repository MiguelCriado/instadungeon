﻿using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using KDTree;

public class ShapeGenerator : MonoBehaviour {
	public delegate char PlaceWallRule(int x, int y);

	public const char WALL_CHAR = '\u25A0';
	public const char FLOOR_CHAR = '\u25A1';
    public const int EMPTY = 0;
	public const int WALL = 1;
    public const int FIXED_FLOOR = 2;
	public const int FLOOR = 3;
	public const int MIN_SURROUNDING_WALLS = 5;

	public float initialWallProb = 0.4f;
	public int iterations = 4;
	public int refiningIterations = 3;
	public int height = 30;
	public int width = 30;

	public int[,] mShape;

    private List<Vector2> fixedFloor;
	

    /// <summary>
    /// Convenient inner class to store lists of cells. Tipically containing a cavern system. 
    /// </summary>
	private struct CellList {
		public int id;
		public List<Vector2> cells;

		public CellList (int id, List<Vector2> cells) {
			this.id = id;
			this.cells = cells;
		}

		public static int CompareByListLength(CellList item1, CellList item2) {
			return item1.cells.Count.CompareTo(item2.cells.Count);
		}

	}

	public void Init(float InitialWallProb, int iterations, int height, int width, List<Vector2> fixedFloor) {
		this.initialWallProb = InitialWallProb;
		this.iterations = iterations;
		this.height = height;
		this.width = width;
        this.fixedFloor = fixedFloor;
	}

    /// <summary>
    /// Generates a new Shape;
    /// </summary>
    /// <returns>The Shape generated</returns>
    public int[,] Generate()
    {
        InitializeShape();
        for (int i = 0; i < iterations; i++)
        {
            mShape = Step(1, 1, 5, 2, 2, 2);
        }
        for (int i = 0; i < refiningIterations; i++)
        {
            mShape = Step(1, 1, 5, 2, 2, -1);
        }
        IdentifyCaverns();
        //Debug.Log(PrintCavernList(ListCaverns()));
        ConnectCaverns();
        CleanUpShape();
        return mShape;
    }

    /// <summary>
    /// Initializes the shape with input parameters. 
    /// </summary>
	private void InitializeShape() {
		float wallCount = 0f;
		mShape = new int[width, height];
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				if (Random.Range(0f, 1f) <= initialWallProb) {
					mShape[i,j] = WALL;
					wallCount++;
				} else {
					mShape[i,j] = FLOOR;
				}
			}
		}
        if (fixedFloor != null) {
            for (int i = 0; i < fixedFloor.Count; i++)
            {
                // Debug.Log("Placing FixedFloor in " + fixedFloor[i]);
                mShape[(int)fixedFloor[i].x, (int)fixedFloor[i].y] = FIXED_FLOOR;
            }
        }
	}

    /// <summary>
    /// Iterates once over the shape applying the cellular automata rules provided. 
    /// </summary>
    /// <param name="scopeX1"></param>
    /// <param name="scopeY1"></param>
    /// <param name="upperThreshold"></param>
    /// <param name="scopeX2"></param>
    /// <param name="scopeY2"></param>
    /// <param name="lowerThreshold"></param>
    /// <returns></returns>
	public int[,] Step(int scopeX1, int scopeY1, int upperThreshold, int scopeX2, int scopeY2, int lowerThreshold) {
		int[,] result = new int[width, height];
		for (int i = 0; i < mShape.GetLength(0); i++) {
			for (int j = 0; j < mShape.GetLength(1); j++) {
				result[i,j] = PlaceWallLogic(i, j, scopeX1, scopeY1, upperThreshold, scopeX2, scopeY2, lowerThreshold);
			}
		}
		return result;
	}

    /// <summary>
    /// Places a Wall tile or a Floor tile based on the cellular automata rule provided. 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="scopeX1"></param>
    /// <param name="scopeY1"></param>
    /// <param name="upperThreshold"></param>
    /// <param name="scopeX2"></param>
    /// <param name="scopeY2"></param>
    /// <param name="lowerThreshold"></param>
    /// <returns></returns>
	private int PlaceWallLogic(int x, int y, int scopeX1, int scopeY1, int upperThreshold, int scopeX2, int scopeY2, int lowerThreshold) {
		int result = FLOOR;
        if (mShape[x, y] != FIXED_FLOOR)
        {
            int numWalls1 = CountSurroundingWalls(x, y, scopeX1, scopeY1);
            int numWalls2 = CountSurroundingWalls(x, y, scopeX2, scopeY2);
            if (IsBound(x, y) ||
                (mShape[x, y] == WALL && (numWalls1 >= upperThreshold - 1 || numWalls2 <= lowerThreshold - 1)) ||
                (numWalls1 >= upperThreshold || numWalls2 <= lowerThreshold))
            {
                result = WALL;
            }
        }
        else
        {
            result = FIXED_FLOOR;
        }
		return result;
	}

    /// <summary>
    ///  Counts how many walls are there around the tile described by [x, y] on the current Shape. It scopes [scopeX, scopeY] tiles away from the tile 
    /// </summary>
    /// <param name="x"></param> x component of the tile we are checking. 
    /// <param name="y"></param> y component of the tile we are checking. 
    /// <param name="scopeX"></param> number of tiles away we are scoping horizontally. 
    /// <param name="scopeY"></param> number of tiles away we are scoping vertically. 
    /// <returns></returns>
	private int CountSurroundingWalls(int x, int y, int scopeX, int scopeY) {
		int result = 0;
		int xInit, yInit, xFinal, yFinal;
		xInit = x - scopeX;
		yInit = y - scopeY;
		xFinal = x + scopeX;
		yFinal = y + scopeY;
		for (int i = xInit; i < xFinal+1; i++) {
			for (int j = yInit; j < yFinal+1; j++){
				if (IsWall(i,j)) {
					result++;
				}
			}
		}
		return result;
	}

	private bool IsWall(int x, int y) {
		return IsOutOfBounds(x, y) || mShape[x,y] == WALL;
	}

	private bool IsOutOfBounds(int x, int y) {
		return x < 0 || x >= mShape.GetLength(0) || y < 0 || y >= mShape.GetLength(1);
	}

	private bool IsBound(int x, int y) {
		return x == 0 || y == 0 || x == mShape.GetLength(0)-1 || y == mShape.GetLength(1)-1;
	}

	private int[,] IdentifyCaverns() {
		int cont = FLOOR + 1;
		for (int i = 0; i < mShape.GetLength(0); i++) {
			for (int j = 0; j < mShape.GetLength(1); j++) {
				if (mShape[i,j] == FLOOR) {
					if (FloodFillIsle(i, j, FLOOR, cont) < 2) {
                        // Debug.Log ("Filling isle " + cont);
						FloodFillIsle(i, j, cont, WALL);
					} else {
						cont++;
					}
                }
                else if (mShape[i, j] == FIXED_FLOOR)
                {
                    if (FloodFillIsle(i, j, FIXED_FLOOR, cont) < 2) {
                        // Debug.Log ("Filling isle " + cont);
						FloodFillIsle(i, j, cont, WALL);
					} else {
						cont++;
					}
                }
			}
		}
		return mShape;
	}
	
	private int FloodFillIsle(int x, int y, int previousValue, int value) {
		mShape[x,y] = value;
		int cont = 1;
		if (!IsOutOfBounds(x, y+1) && mShape[x,y+1] == previousValue) {
			cont += FloodFillIsle(x, y+1, previousValue, value);
		}
        if (!IsOutOfBounds(x+1, y) && mShape[x + 1, y] == previousValue)
        {
			cont += FloodFillIsle(x+1, y, previousValue, value);
		}
        if (!IsOutOfBounds(x, y-1) && mShape[x, y - 1] == previousValue)
        {
			cont += FloodFillIsle(x, y-1, previousValue, value);
		}
        if (!IsOutOfBounds(x-1, y) && mShape[x - 1, y] == previousValue)
        {
			cont += FloodFillIsle(x-1, y, previousValue, value);
		}
		return cont;
	}

	private string MapToString() {
		StringBuilder result = new StringBuilder();
		for (int i = 0; i < this.mShape.GetLength(0); i++) {
			for (int j = 0; j < this.mShape.GetLength(1); j++) {
				result.Append(mShape[i,j]);
			}
			result.AppendLine();
		}
		return result.ToString();
	}

	private List<CellList> ListCaverns() {
		List<CellList> result = new List<CellList>();
		CellList cavernList;
		for (int i = 0; i < mShape.GetLength(0); i++) {
			for (int j = 0; j < mShape.GetLength(1); j++) {
				if (mShape[i,j] > FLOOR) {
					if (!result.Exists(x => x.id == mShape[i,j])) {
						result.Add(new CellList(mShape[i,j], new List<Vector2>()));
					}
					cavernList = result.Find(x => x.id == mShape[i,j]);
					cavernList.cells.Add(new Vector2(i,j));
				}
			}
		}
		result.Sort(CellList.CompareByListLength);
		return result;
	}


    private void ConnectCaverns()
    {
        List<CellList> cavernList = ListCaverns();
        if (cavernList.Count > 1) {
            KDTree<Vector2> tree = new KDTree<Vector2>(2);
            FillTree(cavernList, ref tree);
            Vector2[] nearest;
            for (int i = 0; i < cavernList.Count - 1; i++) {
                nearest = FindNearestTile(tree, cavernList[i].cells);
                // Debug.Log("Gonna connect " + nearest[0] + " with " + nearest[1]);
                MakePath(nearest[0], nearest[1]);
            }
        }
    }

    private void MakePath(Vector2 origin, Vector2 destiny)
    {
        IDTools.PlotFunction plotter = PlacePathPoint;
        IDTools.Line((int)origin.x, (int)origin.y, (int)destiny.x, (int)destiny.y, plotter);
    }

    private bool PlacePathPoint(int x, int y)
    {
        bool result = true;
        if (!IsOutOfBounds(x, y))
        {
            mShape[x, y] = FLOOR;
        } 
        if (!IsOutOfBounds(x-1, y))
        {
            mShape[x-1, y] = FLOOR;
        }
        if (!IsOutOfBounds(x+1, y))
        {
            mShape[x+1, y] = FLOOR;
        }
        if (!IsOutOfBounds(x, y-1))
        {
            mShape[x, y-1] = FLOOR;
        }
        if (!IsOutOfBounds(x, y+1))
        {
            mShape[x, y+1] = FLOOR;
        } 

        return result;
    }


    private static Vector2[] FindNearestTile(KDTree<Vector2> tree, List<Vector2> cells)
    {
        Vector2[] result = new Vector2[2];
        double nearestValue = float.PositiveInfinity;
        double[] point = new double[2];
        NearestNeighbour<Vector2> it;
        for (int i = 0; i < cells.Count; i++)
        {
            point[0] = cells[i].x;
            point[1] = cells[i].y;
            it = tree.NearestNeighbors(point, 1);
            //Debug.Log("Nearest to " + cells[i] + " = " + it.Current);
            while (it.MoveNext())
            {
                if (it.CurrentDistance < nearestValue)
                {
                    nearestValue = it.CurrentDistance;
                    result[0] = cells[i];
                    result[1] = it.Current;
                }
            }
            
        }
        return result;
    }


    private static void FillTree(List<CellList> cavernList, ref KDTree<Vector2> tree)
    {
        CellList biggestCavern = cavernList[cavernList.Count - 1];
        for (int i = 0; i < biggestCavern.cells.Count; i++)
        {
            //Debug.Log("Adding " + biggestCavern.cells[i] + " to the tree");
            tree.AddPoint(new double[] {biggestCavern.cells[i].x, biggestCavern.cells[i].y}, biggestCavern.cells[i]);
        }
       
    }

    private void CleanUpShape()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (mShape[i, j] < FIXED_FLOOR)
                {
                    if (IsNearAFloorTile(i, j))
                    {
                        mShape[i, j] = WALL;
                    }
                    else
                    {
                        mShape[i, j] = EMPTY;
                    }
                }
                // TODO avoid fake passages on diagonals
            }
        }
    }

    private bool IsNearAFloorTile(int x, int y, bool checkDiagonals = false)
    {
        bool result = false;
        if ((!IsOutOfBounds(x-1, y) &&  mShape[x-1, y] >= FIXED_FLOOR)
            || (!IsOutOfBounds(x + 1, y) && mShape[x + 1, y] >= FIXED_FLOOR)
            || (!IsOutOfBounds(x, y - 1) && mShape[x, y - 1] >= FIXED_FLOOR)
            || (!IsOutOfBounds(x, y + 1) && mShape[x, y + 1] >= FIXED_FLOOR))
        {
            result = true;
        }
        if ((checkDiagonals && !result)
            && (!IsOutOfBounds(x - 1, y + 1) && mShape[x - 1, y + 1] >= FIXED_FLOOR)
            && (!IsOutOfBounds(x + 1, y + 1) && mShape[x + 1, y + 1] >= FIXED_FLOOR)
            && (!IsOutOfBounds(x + 1, y - 1) && mShape[x + 1, y - 1] >= FIXED_FLOOR)
            && (!IsOutOfBounds(x - 1, y - 1) && mShape[x - 1, y - 1] >= FIXED_FLOOR))
        {
            result = true;
        }
        return result;
    }

	private string PrintCavernList(List<CellList> caverns) {
		StringBuilder result = new StringBuilder();
		for (int i = 0; i < caverns.Count; i++) {
			result.Append("id = " + caverns[i].id + "\n");
			for (int j = 0; j < caverns[i].cells.Count; j++) {
				result.Append("\t[" + caverns[i].cells[j].x + "," + caverns[i].cells[j].y + "]\n");
			}
		}
		return result.ToString();
	}

}
