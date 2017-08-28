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

	public class GameManager : Manager
	{
		public EventSystem Events { get { return events; } }
		public GameState GameState { get { return gameState; } }
		public Entity Player { get { return player; } }
		public MapManager MapManager { get { return mapManager; } }
		public ITileMapRenderer Renderer { get { return mapRenderer; } }

		private CameraManager cameraManager;
		private EntityManager entityManager;
		private TurnManager turnManager;
		private MapManager mapManager;
		private ITileMapRenderer mapRenderer;
		private MapGenerationManager mapGenerationManager;

		private BehaviorTree turnTree;
		private Blackboard turnBlackboard;

		private EventSystem events;
		private GameState gameState;
		private int floorNumber;
		
		private Entity player;

		public GameManager() : base (false, true)
		{
			events = new EventSystem();
			floorNumber = 0;
		}

		public void Initialize()
		{
			gameState = GameState.Loading;
			mapManager = Locator.Get<MapManager>();
			turnManager = Locator.Get<TurnManager>();
			mapGenerationManager = Locator.Get<MapGenerationManager>();
			entityManager = Locator.Get<EntityManager>();
			cameraManager = Locator.Get<CameraManager>();
			Locator.Get<VisibilityManager>();
			InitializeMapRenderer();
			InitializePlayerCharacter();

			LoadNewMap();
			StartUpTurnSystem();
		}

		public void LoadNewMap(int floorNumber)
		{
			SetState(GameState.Loading);
			this.floorNumber = floorNumber;
			float fadeOutTime = floorNumber > 0 ? 0.5f : 0f;

			turnManager.RevokeControl();

			cameraManager.FadeOut(fadeOutTime)
			.Catch((System.Exception e) =>
			{
				Debug.Log(e.Message);
			})
			.Then(() =>
			{
				TakePlayerFromMap();
				GenerateNewMap(floorNumber);
				PreparePlayerForNewLevel();
				PrepareCameraForNewLevel();

				return cameraManager.FadeIn(0.5f);
			})
			.Catch((System.Exception e) =>
			{
				Debug.Log(e.Message);
			})
			.Done(() =>
			{
				turnManager.GrantControl();
				SetState(GameState.Running);
				this.floorNumber++;
			});
		}

		public void LoadNewMap()
		{
			LoadNewMap(floorNumber);
		}

		public void ResetGame()
		{
			ResetPlayerCharacter();
			LoadNewMap(0);
		}

		public void SetState(GameState state)
		{
			GameState lastState = gameState;
			gameState = state;

			Events.TriggerEvent(new GameStateChangeEvent(lastState, state));
		}

		#region [Event Reactions]

		private void OnPlayerDead(IEventData eventData)
		{
			SetState(GameState.GameOver);
		}

		#endregion

		#region [Helpers]

		protected override void OnUpdate()
		{
			if (gameState == GameState.Running)
			{
				turnTree.Tick(turnManager, turnBlackboard);
			}
		}

		private void GenerateNewMap(int level)
		{
			mapGenerationManager.GenerateNewMap(floorNumber);
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
					new IsGameOverCondition(),
					new IsGamePausedCondition(),
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
