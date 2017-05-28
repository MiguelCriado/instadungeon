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
		public static MapManager MapManager { get { return Instance.mapManager; } }
		public static ITileMapRenderer Renderer { get { return Instance.mapRenderer; } }

		private EntityManager entityManager;
		private TurnManager turnManager;
		private MapManager mapManager;
		private ITileMapRenderer mapRenderer;
		private MapGenerator mapGenerator;
		private CameraManager cameraManager;
		private VisibilityManager visibilityManager;
		private ParticleSystemManager particleSystemManager;

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
			InitializeCameraManager();
			InitializeVisibilityManager();
			InitializeParticleSystemManager();
			InitializePlayerCharacter();
		}

		public static void LoadNewMap()
		{
			Instance.turnManager.RevokeControl();
			Instance.TakePlayerFromMap();
			Instance.GenerateNewMap();
			Instance.PreparePlayerForNewLevel();
			Instance.turnManager.GrantControl();
		}

		private void GenerateNewMap()
		{
			mapGenerator.GenerateNewMap();
			mapRenderer.RenderMap(mapManager.Map);
		}

		private void TakePlayerFromMap()
		{
			if (mapManager.Contains(player))
			{
				mapManager.RemoveActor(player, player.CellTransform.Position);
			}
		}

		private void PreparePlayerForNewLevel()
		{
			mapManager.AddActor(player, mapManager.Map.SpawnPoint);

			TurnComponent playerTurn = player.GetComponent<TurnComponent>();

			if (playerTurn != null)
			{
				turnManager.AddActor(playerTurn);
			}

			cameraManager.Target = player.transform;
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
			mapManager = Locator.Get<MapManager>();
		}

		private void InitializeTurnManager()
		{
			turnManager = Locator.Get<TurnManager>();
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
			entityManager = Locator.Get<EntityManager>();
		}

		private void InitializeVisibilityManager()
		{
			visibilityManager = Locator.Get<VisibilityManager>();
		}

		private void InitializeParticleSystemManager()
		{
			particleSystemManager = Locator.Get<ParticleSystemManager>();
		}

		private void InitializeCameraManager()
		{
			cameraManager = FindObjectOfType<CameraManager>();

			if (cameraManager == null)
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

		#endregion
	}
}
