using InstaDungeon;
using System.Diagnostics;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    public enum LayoutType
    {
        Hilbert
    }
    
    public enum ShapeType
    {
        Cavernous
    }

    public LayoutType layoutType;
    public ShapeType shapeType;
    public bool customSeed = false;
    public int levelSeed;

	private IZoneGenerator shapeGenerator;
    private ILayoutGenerator layoutGenerator;

	public TileMap<Cell> GenerateNewMap()
	{
		TileMap<TileType> blueprint = GenerateBlueprint();
		return GenerateWorld(blueprint);
	}

    public TileMap<TileType> GenerateBlueprint()
    {
		TileMap<TileType> result = null;

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

	private TileMap<Cell> GenerateWorld(TileMap<TileType> blueprint)
	{
		TileMap<Cell> map = GenerateMap(blueprint);
		MapManager mapManager = Locator.Get<MapManager>();
		mapManager.Initialize(map);
		AddEntities(map);
		return map;
	}

	private TileMap<Cell> GenerateMap(TileMap<TileType> blueprint)
	{
		TileMap<Cell> result = blueprint.Convert((TileType cellType) =>
		{
			bool walkable = true;

			if (cellType == TileType.Wall)
			{
				walkable = false;
			}

			return new Cell(new TileInfo(cellType, walkable));
		});

		return result;
	}

	private void AddEntities(TileMap<Cell> map)
	{
		MapManager mapManager = Locator.Get<MapManager>();
		IPropGenerator propGenerator = new BasicPropGenerator();

		propGenerator.AddStairs(mapManager);
		propGenerator.AddDoors(mapManager);
		propGenerator.AddKeys(mapManager);
		// propGenerator.AddItems(mapManager);
	}
}
