using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System.Text;

[RequireComponent(typeof(LayoutGenerator), typeof(ShapeGenerator))]
public class MapHandler : MonoBehaviour {

	[SerializeField] GameObject floorPrefab;

    public Tile[,] Map;

    public SquareGrid PathFindingMap;

	private ShapeGenerator shapeGenerator;
    private LayoutGenerator layoutGenerator;

    private class Restrictions
    {
        public List<Vector2> EastRestrictions;
        public List<Vector2> NorthRestrictions;

        public Restrictions()
        {
            EastRestrictions = new List<Vector2>();
            NorthRestrictions = new List<Vector2>();
        }
    }

	// Use this for initialization
	void Start () {
		shapeGenerator = GetComponent<ShapeGenerator>();
        layoutGenerator = GetComponent<LayoutGenerator>();
        Generate();
	}

    public void Generate()
    {
        RecycleMap();
        if (layoutGenerator && shapeGenerator)
        {
            Stopwatch sw = Stopwatch.StartNew();
            long elapsedMs, lastElapsedMs;
            int[,] map = new int[layoutGenerator.width * shapeGenerator.width, layoutGenerator.height * shapeGenerator.height];
            
            LayoutGenerator.Connections[,] layout = layoutGenerator.Generate();
            elapsedMs = sw.ElapsedMilliseconds;
            UnityEngine.Debug.Log("Time to generate Layout: " + elapsedMs + "ms");
            lastElapsedMs = elapsedMs;
            Restrictions[,] layoutRestrictions = new Restrictions[layoutGenerator.width, layoutGenerator.height];

            for (int i = 0; i < layoutGenerator.width; i++)
            {
                for (int j = 0; j < layoutGenerator.height; j++)
                {
                    // UnityEngine.Debug.Log("### Generating shape = (" + i + ", " + j + ") #################");
                    List<Vector2> currentRestrictions = new List<Vector2>();
                    layoutRestrictions[i, j] = new Restrictions();
                    if ((layout[i, j] & LayoutGenerator.Connections.East) == LayoutGenerator.Connections.East)
                    {
                        // UnityEngine.Debug.Log("Adding East restrictions");
                        Vector2 restriction = new Vector2(shapeGenerator.width - 1, Random.Range(1, shapeGenerator.height - 2));
                        layoutRestrictions[i, j].EastRestrictions.Add(restriction);
                        layoutRestrictions[i, j].EastRestrictions.Add(new Vector2(restriction.x, restriction.y + 1));
                        AddAll(layoutRestrictions[i, j].EastRestrictions, currentRestrictions);
                    }
                    if ((layout[i, j] & LayoutGenerator.Connections.North) == LayoutGenerator.Connections.North)
                    {
                        // UnityEngine.Debug.Log("Adding North restrictions");
                        Vector2 restriction = new Vector2(Random.Range(1, shapeGenerator.width - 2), shapeGenerator.height -1);
                        layoutRestrictions[i, j].NorthRestrictions.Add(restriction);
                        layoutRestrictions[i, j].NorthRestrictions.Add(new Vector2(restriction.x + 1, restriction.y));
                        AddAll(layoutRestrictions[i, j].NorthRestrictions, currentRestrictions);
                    }
                    if (i > 0)
                    {
                        List<Vector2> leftRestrictions = GetCorrespondantRestrictions(layoutRestrictions[i-1, j].EastRestrictions, LayoutGenerator.Connections.West);
                        AddAll(leftRestrictions, currentRestrictions);
                    }
                    if (j > 0)
                    {
                        List<Vector2> bottomRestrictions = GetCorrespondantRestrictions(layoutRestrictions[i, j - 1].NorthRestrictions, LayoutGenerator.Connections.South);
                        AddAll(bottomRestrictions, currentRestrictions);
                    }
                    shapeGenerator.Init(shapeGenerator.initialWallProb, shapeGenerator.iterations, shapeGenerator.height, shapeGenerator.width, currentRestrictions);
                    CopyShape(ref map, shapeGenerator.Generate(), new Vector2(i * shapeGenerator.width, j * shapeGenerator.height));
                }
            }

            Map = PopulateWorld(map);
            
            sw.Stop();
            elapsedMs = sw.ElapsedMilliseconds;
            UnityEngine.Debug.Log("Time to generate: " + elapsedMs + "ms");
        }
    }

    private void RecycleMap()
    {
        if (Map != null)
        {
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }

	private Tile[,] PopulateWorld(int[,] map) {
        Tile[,] result = new Tile[map.GetLength(0), map.GetLength(1)];
        PathFindingMap = new SquareGrid(layoutGenerator.width * shapeGenerator.width, layoutGenerator.height * shapeGenerator.height);
		GameObject aux;
        GameObject tileAux;
		for (int i = 0; i < map.GetLength(0); i++) {
			for (int j = 0; j < map.GetLength(1); j++) {
				if (map[i,j] >= ShapeGenerator.FIXED_FLOOR) {
                    StringBuilder namer = new StringBuilder("tile (");
                    namer.Append(i).Append(", ").Append(j).Append(")");
                    tileAux = new GameObject(namer.ToString());
                    tileAux.AddComponent<Tile>();
                    tileAux.transform.SetParent(this.transform);
                    tileAux.transform.position = IDTools.CartesianToIso(i, j);
					aux = (GameObject)Instantiate(floorPrefab);
					// aux.transform.SetParent(this.transform);
					// aux.transform.position = IDTools.CartesianToIso(i, j);
                    tileAux.GetComponent<Tile>().AddEntity(aux);
                    aux.transform.position = IDTools.CartesianToIso(i, j);
                    result[i, j] = tileAux.GetComponent<Tile>();
                    PathFindingMap.floorTiles.Add(new Vector2Int(i, j));
				}
			}
		}
       
        return result;
	}

	// Update is called once per frame
	void Update () {
	
	}

    private void AddAll(List<Vector2> origin, List<Vector2> destiny)
    {
        for (int i = 0; i < origin.Count; i++)
        {
            destiny.Add(origin[i]);
        }
    }

    private List<Vector2> GetCorrespondantRestrictions(List<Vector2> origin, LayoutGenerator.Connections relativePosition)
    {
        List<Vector2> result = new List<Vector2>();
        if ((relativePosition & LayoutGenerator.Connections.West) == LayoutGenerator.Connections.West)
        {
            for (int i = 0; i < origin.Count; i++)
            {
                result.Add(new Vector2(0, origin[i].y));
            }
        }
        if ((relativePosition & LayoutGenerator.Connections.South) == LayoutGenerator.Connections.South)
        {
            for (int i = 0; i < origin.Count; i++)
            {
                result.Add(new Vector2(origin[i].x, 0));
            }
        }
        return result;
    }

    private void CopyShape(ref int[,] map, int[,] shape, Vector2 offset)
    {
        for (int i = 0; i < shapeGenerator.width; i++)
        {
            for (int j = 0; j < shapeGenerator.height; j++)
            {
                map[(int)offset.x + i, (int)offset.y + j] = shape[i, j];
            }
        }
    }
}
