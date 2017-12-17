using DG.Tweening;
using InstaDungeon.Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace InstaDungeon.UI
{
	public class PausePanelController : MonoBehaviour
	{
		[Header("References")]
		[SerializeField] private RectTransform content;
		[SerializeField] private RectTransform optionSelector;
		[SerializeField] private MenuOption continueOption;
		[SerializeField] private MenuOption retryOption;
		[SerializeField] private MenuOption mainMenuOption;
		[SerializeField] private Text levelSeed;
		[Header("Settings")]
		[SerializeField] private KeyCode pauseKey;
		[SerializeField] private string mainMenu;

		private GameManager gameManager;
		private MapGenerationManager generationManager;
		private MenuOption currentSelectedOption;

		private void Reset()
		{
			pauseKey = KeyCode.Escape;
			mainMenu = "Main Menu";
		}

		private void Awake()
		{
			gameManager = Locator.Get<GameManager>();
			gameManager.Events.AddListener(OnGameStateChange, GameStateChangeEvent.EVENT_TYPE);

			generationManager = Locator.Get<MapGenerationManager>();
			generationManager.Events.AddListener(OnMapGenerated, MapGenerationNewMapEvent.EVENT_TYPE);

			continueOption.OnOptionSelected.AddListener(() => 
			{
				MoveSelector(continueOption.GetComponent<RectTransform>());
				SetSelectedOption(continueOption);
			});

			continueOption.OnOptionPressed.AddListener(() => 
			{
				gameManager.SetState(GameState.Running);
			});

			retryOption.OnOptionSelected.AddListener(() =>
			{
				MoveSelector(retryOption.GetComponent<RectTransform>());
				SetSelectedOption(retryOption);
			});

			retryOption.OnOptionPressed.AddListener(() =>
			{
				gameManager.ResetGame();
			});

			mainMenuOption.OnOptionSelected.AddListener(() =>
			{
				MoveSelector(mainMenuOption.GetComponent<RectTransform>());
				SetSelectedOption(mainMenuOption);
			});

			mainMenuOption.OnOptionPressed.AddListener(() =>
			{
				SceneManager.LoadScene(mainMenu);
			});
		}

		private void Start()
		{
			content.gameObject.SetActive(false);
		}

		private void Update()
		{
			if (Input.GetKeyDown(pauseKey) && gameManager.GameState == GameState.Running)
			{
				gameManager.SetState(GameState.Paused);
			}
		}

		public void BackgroundSelected()
		{
			if (currentSelectedOption != null)
			{
				currentSelectedOption.Select();
			}
		}

		private void OnGameStateChange(IEventData eventData)
		{
			GameStateChangeEvent stateEvent = eventData as GameStateChangeEvent;

			if (stateEvent.State == GameState.Paused)
			{
				OnPause();
			}
			else
			{
				HideMenu();
			}
		}

		private void OnMapGenerated(IEventData eventData)
		{
			MapGenerationNewMapEvent mapEvent = eventData as MapGenerationNewMapEvent;
			levelSeed.text = mapEvent.LevelSeed.ToString();
		}

		private void OnPause()
		{
			content.gameObject.SetActive(true);
			Canvas.ForceUpdateCanvases();
			continueOption.Select();
		}

		private void HideMenu()
		{
			if (content.gameObject.activeInHierarchy)
			{
				content.gameObject.SetActive(false);
			}
		}

		private void MoveSelector(RectTransform transform)
		{
			DOTween.Kill(optionSelector);
			optionSelector.DOMove(transform.position, 0.15f).SetEase(Ease.InOutSine);
		}

		private void SetSelectedOption(MenuOption menuOption)
		{
			currentSelectedOption = menuOption;
		}
	}
}
