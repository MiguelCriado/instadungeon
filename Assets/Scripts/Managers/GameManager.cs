using AI.BehaviorTrees;
using InstaDungeon.BehaviorTreeNodes;
using InstaDungeon.Components;
using InstaDungeon.Events;
using UnityEngine;

namespace InstaDungeon
{
	public enum GameState
	{
		Loading,
		Running,
		Paused,
		GameOver
	}

	public class GameManager : Singleton<GameManager>
	{
		public static EventSystem Events { get { return Instance.events; } }
		public static GameState GameState { get { return Instance.gameState; } }
		public static Entity Player { get { return Instance.player; } }
		public static MapManager MapManager { get { return Instance.mapManager; } }
		public static ITileMapRenderer Renderer { get { return Instance.mapRenderer; } }

		private CameraManager cameraManager;
		private EntityManager entityManager;
		private TurnManager turnManager;
		private MapManager mapManager;
		private ITileMapRenderer mapRenderer;
		private MapGenerator mapGenerator;
		private VisibilityManager visibilityManager;
		private ParticleSystemManager particleSystemManager;

		private BehaviorTree turnTree;
		private Blackboard turnBlackboard;

		private EventSystem events;
		private GameState gameState;
		private int floorNumber;
		
		private Entity player;

		private void Awake()
		{
			events = new EventSystem();
		}

		private void Start()
		{
			Initialize();
			LoadNewMap();
			StartUpTurnSystem();
		}

		private void Update()
		{
			if (gameState == GameState.Running)
			{
				turnTree.Tick(turnManager, turnBlackboard);
			}
		}

		public void Initialize()
		{
			gameState = GameState.Loading;
			InitializeMapManager();
			InitializeTurnManager();
			InitializeMapGenerator();
			InitializeMapRenderer();
			InitializeEntityManager();
			InitializeCameraManager();
			InitializeVisibilityManager();
			InitializeParticleSystemManager();
			InitializePlayerCharacter();
			floorNumber = 0;
		}

		public static void LoadNewMap()
		{
			Instance.LoadNewMapInternal();
		}

		public static void ResetGame()
		{
			Instance.ResetPlayerCharacter();
			Instance.LoadNewMapInternal(0);
		}

		public static void SetState(GameState state)
		{
			GameState lastState = Instance.gameState;
			Instance.gameState = state;

			Events.TriggerEvent(new GameStateChangeEvent(lastState, state));
		}

		#region [Event Reactions]

		private void OnPlayerDead(IEventData eventData)
		{
			EntityDieEvent deathEvent = eventData as EntityDieEvent;
			SetState(GameState.GameOver);
		}

		#endregion

		#region [Helpers]

		private void LoadNewMapInternal(int floorNumber)
		{
			SetState(GameState.Loading);
			this.floorNumber = floorNumber;
			float fadeOutTime = floorNumber > 0 ? 0.5f : 0f;

			turnManager.RevokeControl();

			cameraManager.FadeOut(fadeOutTime)
			.Catch((System.Exception e) => 
			{
				throw e;
			})
			.Then(() =>
			{
				TakePlayerFromMap();
				GenerateNewMap(); // TODO take floor number
				PreparePlayerForNewLevel();
				PrepareCameraForNewLevel();
				
				return cameraManager.FadeIn(0.5f);
			})
			.Done(() => 
			{
				turnManager.GrantControl();
				SetState(GameState.Running);
				floorNumber++;
			});
		}

		private void LoadNewMapInternal()
		{
			LoadNewMapInternal(floorNumber);
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
		}

		private void PrepareCameraForNewLevel()
		{
			cameraManager.SetTarget(player.transform);
			cameraManager.MoveTo(player.transform.position);
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

		#endregion

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
			cameraManager = Locator.Get<CameraManager>();
		}

		private void InitializePlayerCharacter()
		{
			if (player == null)
			{
				player = entityManager.Spawn("Player");
				player.Events.AddListener(OnPlayerDead, EntityDieEvent.EVENT_TYPE);
			}
		}

		private void ResetPlayerCharacter()
		{
			player.GetComponent<Health>().ResetComponent();
			// TODO reset inventory and other stuff
		}

		#endregion
	}
}
