using AI.BehaviorTrees;
using InstaDungeon.BehaviorTreeNodes;
using InstaDungeon.Commands;
using InstaDungeon.Components;
using UnityEngine;

namespace InstaDungeon
{
	public class GameManager : Singleton<GameManager>
	{
		public static Entity Player { get { return Instance.player; } }
		public static CameraManager Camera { get { return Instance.cameraManager; } }

		private EntityManager entityManager;
		private TurnManager turnManager;
		private MapManager mapManager;
		private ITileMapRenderer mapRenderer;
		private MapGenerator mapGenerator;
		private CameraManager cameraManager;

		private BehaviorTree turnTree;
		private Blackboard turnBlackboard;

		private Entity player;

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

		public void Initialize()
		{
			InitializeMapManager();
			InitializeTurnManager();
			InitializeMapGenerator();
			InitializeMapRenderer();
			InitializeEntityManager();
			InitializePlayerCharacter();
			InitializeCameraManager();
		}

		public static void LoadNewMap()
		{
			Instance.GenerateNewMap();
			Instance.PreparePlayerForNewLevel();
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

		private void PreparePlayerForNewLevel()
		{
			CellTransform playerCell = player.GetComponent<CellTransform>();

			mapManager.Spawn(new MoveActorCommand(playerCell, mapManager.Map.SpawnPoint));

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

		#region Initialization

		private void InitializeMapManager()
		{
			mapManager = new MapManager();
		}

		private void InitializeTurnManager()
		{
			turnManager = new TurnManager();
		}

		private void InitializeMapGenerator()
		{
			mapGenerator = GetComponentInChildren<MapGenerator>();

			if (mapGenerator == null)
			{
				Locator.Log.Error("There must be an object of type MapGenerator as a child of " + gameObject.name);
			}
		}

		private void InitializeMapRenderer()
		{
			GameObject mapRendererObject = GameObject.FindGameObjectWithTag("TileMapRenderer");

			if (mapRendererObject != null)
			{
				mapRenderer = mapRendererObject.GetComponent<ITileMapRenderer>();

				if (mapRenderer == null)
				{
					Locator.Log.Error("The GameObject with tag \"TileMapRenderer\" must contains an ITileMapRenderer component.");
				}
			}
			else
			{
				Locator.Log.Error("There must be one GameObject with tag \"TileMapRenderer\" in the scene.");
			}
		}

		private void InitializeEntityManager()
		{
			entityManager = FindObjectOfType<EntityManager>();

			if (entityManager == null)
			{
				Locator.Log.Error("There must be an object of type EntityManager in the scene.");
			}
		}

		private void InitializePlayerCharacter()
		{
			if (player == null)
			{
				player = entityManager.Spawn("Player");
			}
		}

		private void InitializeCameraManager()
		{
			cameraManager = FindObjectOfType<CameraManager>();

			if (cameraManager != null)
			{
				cameraManager.Target = player.transform;
			}
			else
			{
				Locator.Log.Error("There must be an object of type EntityManager in the scene.");
			}
		}

		#endregion
	}
}
