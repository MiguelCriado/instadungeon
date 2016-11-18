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

	void Start ()
	{
		// GenerateNewMap();
	}

	public void GenerateNewMap()
	{
		TileMap<TileType> blueprint = GenerateBlueprint();
		GenerateWorld(blueprint);
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

	private void GenerateWorld(TileMap<TileType> blueprint)
	{
		Stopwatch sw = Stopwatch.StartNew();

		TileMap<Cell> actualMap = blueprint.Convert((TileType cellType) => 
		{
			return new Cell(new TileInfo(cellType));
		});

		ITileMapRenderer tileMapRenderer = GetComponent<ITileMapRenderer>();

		if (tileMapRenderer != null)
		{
			tileMapRenderer.RenderMap(actualMap);

			sw.Stop();
			UnityEngine.Debug.Log("Time to generate world: " + sw.ElapsedMilliseconds + "ms");
		}
		else
		{
			sw.Stop();
			UnityEngine.Debug.Log("ITileMapRenderer not found");
		}
	}
}
