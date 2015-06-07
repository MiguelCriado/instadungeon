using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System.Text;

[RequireComponent(typeof(HilbertLayoutGenerator), typeof(CavernousShapeGenerator))]
public class MapHandler : MonoBehaviour {

	[SerializeField] 
    GameObject floorPrefab;
    [SerializeField]
    GameObject wallPrefab;

    public Map<Tile> Map;

    public SquareGrid PathFindingMap;

	private CavernousShapeGenerator shapeGenerator;
    private HilbertLayoutGenerator layoutGenerator;

    

	// Use this for initialization
	void Start () {
		shapeGenerator = GetComponent<CavernousShapeGenerator>();
        layoutGenerator = GetComponent<HilbertLayoutGenerator>();
        Generate();
	}

    public void Generate()
    {
        RecycleMap();
        if (layoutGenerator && shapeGenerator)
        {
            Stopwatch sw = Stopwatch.StartNew();
            long elapsedMs, lastElapsedMs;

            Map<BlueprintAsset> blueprintMap = ShapeConnector.BuildMap(layoutGenerator, shapeGenerator);


            Map = PopulateWorld(blueprintMap);
            
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

	private Map<Tile> PopulateWorld(Map<BlueprintAsset> map) {
        Map<Tile> result = new Map<Tile>();
		GameObject aux = null;
        GameObject tileAux = null;

        foreach (KeyValuePair<Vector2Int, BlueprintAsset> tile in map)
        {
            BlueprintAsset asset = tile.Value;
            Vector2Int position = tile.Key;
            StringBuilder namer = new StringBuilder("tile (");
            namer.Append(position.x).Append(", ").Append(position.y).Append(")");
            tileAux = new GameObject(namer.ToString());
            tileAux.AddComponent<Tile>();
            tileAux.transform.SetParent(this.transform);
            tileAux.transform.position = IDTools.CartesianToIso(position.x, position.y);
            switch (asset) {
                case BlueprintAsset.Floor:
                    aux = (GameObject)Instantiate(floorPrefab);
                    break;
                case BlueprintAsset.Wall:
                    aux = (GameObject)Instantiate(wallPrefab);
                    break;
            }
            tileAux.GetComponent<Tile>().AddEntity(aux);
            result.Add(position, tileAux.GetComponent<Tile>());
            aux.transform.position = IDTools.CartesianToIso(position.x, position.y);
            // PathFindingMap.floorTiles.Add(new Vector2Int(position.x, position.y));
        }
		return result;
	}

	// Update is called once per frame
	void Update () {
	
	}
}
