using InstaDungeon.Components;
using InstaDungeon.Events;
using UnityEngine;
using UnityEngine.UI;

namespace InstaDungeon.UI
{
	public class HealthControllerHUD : MonoBehaviour
	{
		private static readonly string HealthFormat = "{0}/{1}";

		[Header("References")]
		[SerializeField] private Slider healthSlider;
		[SerializeField] private Text healthText;

		private void Start()
		{
			LoadInitialData();
			SubscribeEvents();
		}

		#region [Event Handlers]

		private void OnPlayerHealthChange(IEventData eventData)
		{
			EntityHealthChangeEvent healthEvent = eventData as EntityHealthChangeEvent;
			RefreshBar(healthEvent.Health.MaxHealth, healthEvent.CurrentHealth);
		}

		#endregion

		#region [Helpers]

		private void LoadInitialData()
		{
			Health health = Locator.Get<GameManager>().Player.GetComponent<Health>();
			RefreshBar(health.MaxHealth, health.CurrentHealth);
		}

		private void SubscribeEvents()
		{
			Locator.Get<GameManager>().Player.Events.AddListener(OnPlayerHealthChange, EntityHealthChangeEvent.EVENT_TYPE);
		}

		private void RefreshBar(int maxHealth, int currentHealth)
		{
			healthSlider.maxValue = maxHealth;
			healthSlider.value = currentHealth;
			healthText.text = string.Format(HealthFormat, currentHealth, maxHealth);
		}

		#endregion
	}
}
