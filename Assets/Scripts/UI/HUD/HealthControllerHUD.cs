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

		private Entity entity;

		public void Initialize(Entity entity)
		{
			this.entity = entity;
			Refresh();
			SubscribeEvents(entity);
		}

		#region [Event Handlers]

		private void OnPlayerHealthChange(IEventData eventData)
		{
			EntityHealthChangeEvent healthEvent = eventData as EntityHealthChangeEvent;
			RefreshBar(healthEvent.Health.MaxHealth, healthEvent.CurrentHealth);
		}

		#endregion

		#region [Helpers]

		private void SubscribeEvents(Entity entity)
		{
			entity.Events.AddListener(OnPlayerHealthChange, EntityHealthChangeEvent.EVENT_TYPE);
		}

		private void Refresh()
		{
			if (entity != null)
			{
				Health health = entity.GetComponent<Health>();

				if (health != null)
				{
					RefreshBar(health.MaxHealth, health.CurrentHealth);
				}
			}
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
