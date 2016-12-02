using AI.BehaviorTrees;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	public CellTransform player; // TODO: remove this from here

	private MapManager mapManager;
	private TurnManager turnManager;
	private ITileMapRenderer mapRenderer;
	private MapGenerator mapGenerator;

	private BehaviorTree turnTree;
	private Blackboard turnBlackboard;

	void Start()
	{
		Initialize();
		LoadNewMap();
		StartUpTurnSystem();
	}

	void Update()
	{
		turnTree.Tick(turnManager, turnBlackboard);
	}

	public static void LoadNewMap()
	{
		Instance.GenerateNewMap();
		Instance.InitializePlayer();
	}

	public static bool MoveActor(MoveActorCommand moveCommand)
	{
		bool result = false;

		if (Instance.mapManager.MoveTo(moveCommand))
		{
			moveCommand.Execute();
			result = true;
		}

		return result;
	}

	public static Vector3 CellToWorld(int2 position)
	{
		return Instance.mapRenderer.SnappedTileMapToWorldPosition(position);
	}

	public static Cell GetCell(int x, int y)
	{
		return Instance.mapManager[x, y];
	}

	public void Initialize()
	{
		mapManager = new MapManager();
		turnManager = new TurnManager();

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
			bool walkable = true;

			if (cellType == TileType.Wall)
			{
				walkable = false;
			}

			return new TileInfo(cellType, walkable);
		}); // TODO: work a way to avoid this conversion

		mapManager.Initialize(tileInfoMap);
		mapRenderer.RenderMap(mapManager.Map);
	}

	public void InitializePlayer()
	{
		mapManager.Spawn(new MoveActorCommand(player, mapManager.Map.SpawnPoint));

		TurnComponent playerTurn = player.GetComponent<TurnComponent>();

		if (playerTurn != null)
		{
			turnManager.AddActor(playerTurn);
		}
	}

	private void StartUpTurnSystem()
	{
		turnBlackboard = new Blackboard();

		turnTree = new BehaviorTree
			(
				new Priority
				(
					new Sequence
					(
						new Inverter(new IsLevelCompletedCondition()),
						new ManageTurnAction()
					),
					new LoadNewLevelAction()
				)
				
			);

		turnManager.Init();
	}
}
