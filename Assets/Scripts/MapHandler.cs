using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System.Text;

public class MapHandler : MonoBehaviour {

    public enum LayoutType
    {
        Hilbert
    }
    
    public enum ShapeType
    {
        Cavernous
    }

    public enum DungeonType
    {
        Isometric, 
        Orthogonal, 
        _3D
    }

    public DungeonType dungeonType;
    public LayoutType layoutType;
    public ShapeType shapeType;

    float isometricTileSize = 1;

    public bool customSeed = false;
    public int levelSeed;

    public GameObject floorPrefab;    
    public GameObject wallPrefab;
    public GameObject entranceStairs;
    public GameObject exitStairs;

    public GameObject floorPrefab_iso;
    public GameObject wallPrefab_iso;
    public GameObject entranceStairs_iso;
    public GameObject exitStairs_iso;

    public GameObject floorPrefab_ortho;
    public GameObject wallPrefab_ortho;
    public GameObject entranceStairs_ortho;
    public GameObject exitStairs_ortho;

    public GameObject floorPrefab_3D;
    public GameObject wallPrefab_3D;
    public GameObject entranceStairs_3D;
    public GameObject exitStairs_3D;


    public GameObject player; // TODO REMOVE THIS SHIT OUT OF HERE!!!!

    public Map<Tile> Map;

	private ShapeGenerator shapeGenerator;
    private LayoutGenerator layoutGenerator;



	// Use this for initialization
	void Start () {
        Generate();
	}

    public void Generate()
    {
        AssignPrefabs();
        RecycleMap();

        layoutGenerator = GetComponent<LayoutGenerator>();
        shapeGenerator = GetComponent<ShapeGenerator>();

        if (layoutGenerator != null && shapeGenerator != null)
        {
            Stopwatch sw = Stopwatch.StartNew();
            long elapsedMs, lastElapsedMs;

            if (!customSeed)
            {
                levelSeed = Random.seed;
            }

            Map<BlueprintAsset> blueprintMap = ShapeConnector.BuildMap(layoutGenerator, shapeGenerator, levelSeed);

            lastElapsedMs = sw.ElapsedMilliseconds;
            UnityEngine.Debug.Log("Time to generate blueprint Map: " + lastElapsedMs + "ms");
            Map = PopulateWorld(blueprintMap);
            
            sw.Stop();
            elapsedMs = sw.ElapsedMilliseconds - lastElapsedMs;
            UnityEngine.Debug.Log("Time to populate world: " + elapsedMs + "ms");
            UnityEngine.Debug.Log("Total time to generate: " + sw.ElapsedMilliseconds + "ms");

            PlaceEntrance(Map);
        }
    }

    private void AssignPrefabs()
    {
        switch (dungeonType)
        {
            case DungeonType._3D:
                floorPrefab = floorPrefab_3D;
                wallPrefab = wallPrefab_3D;
                entranceStairs = entranceStairs_3D;
                exitStairs = exitStairs_3D;
                break;
            case DungeonType.Isometric:
                floorPrefab = floorPrefab_iso;
                wallPrefab = wallPrefab_iso;
                entranceStairs = entranceStairs_iso;
                exitStairs = exitStairs_iso;
                break;
            case DungeonType.Orthogonal:
                floorPrefab = floorPrefab_ortho;
                wallPrefab = wallPrefab_ortho;
                entranceStairs = entranceStairs_ortho;
                exitStairs = exitStairs_ortho;
                break;
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
            tileAux.transform.position = CalculatePosition(position, this.dungeonType);
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
            aux.transform.position = CalculatePosition(position, this.dungeonType);
            if (dungeonType != DungeonType._3D)
            {
                CalculateSortingOrder(aux);
            }
            // PathFindingMap.floorTiles.Add(new Vector2Int(position.x, position.y));
        }
        result.GetLayout().InitialZone = map.GetLayout().InitialZone;
        result.GetLayout().FinalZone = map.GetLayout().FinalZone;
        result.spawnPoint = map.spawnPoint;
        result.exitPoint = map.exitPoint;
		return result;
	}

    private void PlaceEntrance(Map<Tile> map)
    {
        player.transform.position = CalculatePosition(map.spawnPoint, this.dungeonType);
        /*foreach (Vector2Int tile in map.GetLayout().InitialZone)
        {
            if (map.GetTile(tile.x, tile.y).Cost() > 0)
            {
                UnityEngine.Debug.Log("Moving player to: \n\t cartesian: " + tile.ToString() + "\n\t isometric: " + IDTools.CartesianToIso(tile.x, tile.y));
                player.transform.position = CalculatePosition(tile, this.dungeonType);
                break;
            }
        }*/
    }

    private void CalculateSortingOrder(GameObject obj)
    {
        SpriteRenderer renderer = (SpriteRenderer)obj.gameObject.GetComponent("SpriteRenderer");
        renderer.sortingOrder = Mathf.FloorToInt((obj.transform.position.y - obj.transform.position.z) * -100);
    }

    private Vector3 CalculatePosition(Vector2Int position, DungeonType dungeonType)
    {
        Vector3 result = new Vector3();
        switch (dungeonType)
        {
            case DungeonType.Isometric:
                result = IDTools.CartesianToIso(position.x, position.y);
                break;
            case DungeonType.Orthogonal:
                result.Set(position.x, position.y, 0f);
                break;
            case DungeonType._3D:
                result.Set(position.x, 0f, position.y);
                break;
        }
        return result;
    }
}
