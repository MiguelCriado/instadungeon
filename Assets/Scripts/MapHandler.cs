using InstaDungeon.TileMap;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapHandler : MonoBehaviour
{
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

    public float isometricTileSize = 1;

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

    public TileMap<TileBehaviour> Map;

	private IZoneGenerator shapeGenerator;
    private ILayoutGenerator layoutGenerator;


	void Start ()
	{
		GenerateNewMap();
	}

	public void GenerateNewMap()
	{
		TileMap<TileType> blueprint = GenerateBlueprint();

		if (GetComponent<OrthogonalTileMapRenderer>() != null)
		{
			GenerateWorld(blueprint);
		}
		else
		{
			Map = PopulateWorld(blueprint);
			PlaceEntrance(Map);
		}
	}

    public TileMap<TileType> GenerateBlueprint()
    {
		TileMap<TileType> result = null;
			 
        AssignPrefabs();
        RecycleMap();

        layoutGenerator = GetComponent<ILayoutGenerator>();
        shapeGenerator = GetComponent<IZoneGenerator>();

        if (layoutGenerator != null && shapeGenerator != null)
        {
            Stopwatch sw = Stopwatch.StartNew();

            if (!customSeed)
            {
				int seed = System.Guid.NewGuid().GetHashCode() ^ System.DateTime.UtcNow.Millisecond;
				Random.InitState(seed);

                levelSeed = seed;
            }

            result = ZoneConnector.BuildMap(layoutGenerator, shapeGenerator, levelSeed);

            UnityEngine.Debug.Log("Time to generate blueprint Map: " + sw.ElapsedMilliseconds + "ms");
        }

		return result;
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

	private void GenerateWorld(TileMap<TileType> blueprint)
	{
		Stopwatch sw = Stopwatch.StartNew();

		int2[] blueprintTiles = blueprint.GetPresentTiles();

		TileMap<TileInfo> actualMap = new TileMap<TileInfo>();

		for (int i = 0; i < blueprintTiles.Length; i++)
		{
			int2 tilePosition = blueprintTiles[i];
			actualMap[tilePosition.x, tilePosition.y] = new TileInfo(blueprint[tilePosition.x, tilePosition.y]);
		}

		OrthogonalTileMapRenderer renderer = GetComponent<OrthogonalTileMapRenderer>();
		renderer.BuildMesh(actualMap);

		sw.Stop();
		UnityEngine.Debug.Log("Time to generate world: " + sw.ElapsedMilliseconds + "ms");
	}

	private TileMap<TileBehaviour> PopulateWorld(TileMap<TileType> map)
	{
		Stopwatch sw = Stopwatch.StartNew();

		TileMap<TileBehaviour> result = new TileMap<TileBehaviour>();
		GameObject aux = null;
        GameObject tileAux = null;

        foreach (KeyValuePair<int2, TileType> tile in map)
        {
            TileType asset = tile.Value;
			int2 position = tile.Key;
            StringBuilder namer = new StringBuilder("tile (");
            namer.Append(position.x).Append(", ").Append(position.y).Append(")");
            tileAux = new GameObject(namer.ToString());
            tileAux.AddComponent<TileBehaviour>();
            tileAux.transform.SetParent(transform);
            tileAux.transform.position = CalculatePosition(position, dungeonType);

            switch (asset)
			{
                case TileType.Floor:
                    aux = Instantiate(floorPrefab);
                    break;
                case TileType.Wall:
                    aux = Instantiate(wallPrefab);
                    break;
            }

            tileAux.GetComponent<TileBehaviour>().AddEntity(aux);
            result.Add(position, tileAux.GetComponent<TileBehaviour>());
            aux.transform.position = CalculatePosition(position, dungeonType);

            if (dungeonType != DungeonType._3D)
            {
                CalculateSortingOrder(aux);
            }
            // PathFindingMap.floorTiles.Add(new Vector2Int(position.x, position.y));
        }

        result.Layout.InitialZone = map.Layout.InitialZone;
        result.Layout.FinalZone = map.Layout.FinalZone;
        result.spawnPoint = map.spawnPoint;
        result.exitPoint = map.exitPoint;

		sw.Stop();
		UnityEngine.Debug.Log("Time to populate world: " + sw.ElapsedMilliseconds + "ms");

		return result;
	}

    private void PlaceEntrance(TileMap<TileBehaviour> map)
    {
        player.transform.position = CalculatePosition(map.spawnPoint, dungeonType);
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

    private Vector3 CalculatePosition(int2 position, DungeonType dungeonType)
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
