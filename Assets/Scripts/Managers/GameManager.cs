using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	private MapManager mapManager;
	private ITileMapRenderer mapRenderer;
	private MapGenerator mapGenerator;

	void Start()
	{
		Initialize();
		GenerateNewMap();
	}

	public void Initialize()
	{
		mapManager = FindObjectOfType<MapManager>();

		GameObject mapRendererObject = GameObject.FindGameObjectWithTag("TileMapRenderer");

		if (mapRendererObject != null)
		{
			mapRenderer = mapRendererObject.GetComponent<ITileMapRenderer>();

			if (mapRenderer == null)
			{
				Debug.LogError("The GameObject with tag \"TileMapRenderer\" must contains an ITileMapRenderer component.");
			}
		}
		else
		{
			Debug.LogError("There must be one GameObject with tag \"TileMapRenderer\" in the scene.");
		}

		mapGenerator = GetComponentInChildren<MapGenerator>();
	}

	public void GenerateNewMap()
	{
		TileMap<TileType> blueprintMap = mapGenerator.GenerateBlueprint();

		TileMap<TileInfo> tileInfoMap = blueprintMap.Convert((TileType cellType) =>
		{
			return new TileInfo(cellType);
		}); // TODO: work a way to avoid this conversion

		mapManager.Initialize(tileInfoMap);
		mapRenderer.RenderMap(mapManager.Map);
	}

	public static void MoveActorTo(GameObject actor, int2 position)
	{
		Instance.mapManager.MoveTo(actor, position);
	}
}
