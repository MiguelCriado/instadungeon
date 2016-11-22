using AI.BehaviorTrees;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	private MapManager mapManager;
	private TurnManager turnManager;
	private ITileMapRenderer mapRenderer;
	private MapGenerator mapGenerator;

	private BehaviorTree turnTree;
	private Blackboard turnBlackboard;

	void Start()
	{
		Initialize();
		GenerateNewMap();
		StartUpTurnSystem();
	}

	void Update()
	{
		turnTree.Tick(turnManager, turnBlackboard);
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
			return new TileInfo(cellType);
		}); // TODO: work a way to avoid this conversion

		mapManager.Initialize(tileInfoMap);
		mapRenderer.RenderMap(mapManager.Map);
	}

	public static void MoveActorTo(GameObject actor, int2 position)
	{
		Instance.mapManager.MoveTo(actor, position);
	}

	private void StartUpTurnSystem()
	{
		GameObject player = GameObject.FindGameObjectWithTag("Player");

		turnManager.AddActor(player);
		turnBlackboard = new Blackboard();

		turnTree = new BehaviorTree
			(
				new Sequence
				(
					new Inverter(new IsLevelCompletedCondition()),
					new ManageTurnAction()
				)
			);

		turnManager.Init();
	}
}
