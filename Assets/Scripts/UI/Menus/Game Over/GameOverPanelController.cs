using DG.Tweening;
using InstaDungeon.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InstaDungeon.UI
{
	public class GameOverPanelController : MonoBehaviour
	{
		[Header("References")]
		[SerializeField] private RectTransform content;
		[SerializeField] private RectTransform optionSelector;
		[SerializeField] private MenuOption retryOption;
		[SerializeField] private MenuOption mainMenuOption;
		[Header("Settings")]
		[SerializeField] private string mainMenu;

		private MenuOption currentSelectedOption;

		private void Reset()
		{
			mainMenu = "Main Menu";
		}

		private void Awake()
		{
			retryOption.OnOptionSelected.AddListener(() => 
			{
				MoveSelector(retryOption.GetComponent<RectTransform>());
				SetSelectedOption(retryOption);
			});

			retryOption.OnOptionPressed.AddListener(() => 
			{
				Locator.Get<GameManager>().ResetGame();
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

		public void Start()
		{
			Locator.Get<GameManager>().Events.AddListener(OnGameStateChange, GameStateChangeEvent.EVENT_TYPE);
			content.gameObject.SetActive(false);
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

			if (stateEvent.State == GameState.GameOver)
			{
				OnGameOver();
			}
			else
			{
				HideMenu();
			}
		}

		private void OnGameOver()
		{
			content.gameObject.SetActive(true);
			Canvas.ForceUpdateCanvases();
			retryOption.Select();
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
