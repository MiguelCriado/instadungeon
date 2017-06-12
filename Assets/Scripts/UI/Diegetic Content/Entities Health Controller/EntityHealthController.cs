using InstaDungeon.Components;
using InstaDungeon.Events;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace InstaDungeon.UI
{
	[RequireComponent(typeof(FollowWorldTransform), typeof(CanvasGroup))]
	public class EntityHealthController : MonoBehaviour
	{
		[SerializeField] private Slider slider;

		private Health health;
		private FollowWorldTransform follower;
		private CanvasGroup canvasGroup;

		private void Reset()
		{
			slider = GetComponentInChildren<Slider>();
		}

		private void Awake()
		{
			follower = GetComponent<FollowWorldTransform>();
			canvasGroup = GetComponent<CanvasGroup>();
			DOTween.Init();
		}

		private void OnDestroy()
		{
			if (health != null)
			{
				health.Entity.Events.RemoveListener(OnHealthChange, EntityHealthChangeEvent.EVENT_TYPE);
				health.Entity.Events.RemoveListener(OnVisibilityChange, EntityVisibilityChangeEvent.EVENT_TYPE);
			}
		}

		public void Initialize(Health health)
		{
			this.health = health;
			RefreshBar(0, health.MaxHealth, health.CurrentHealth);

			follower.Target = health.transform;
			follower.Offset = new Vector2(0, 24f);
			follower.RefreshPosition();

			VisibilityType visibility = Locator.Get<MapManager>().Map[health.Entity.CellTransform.Position].Visibility;

			if (visibility != VisibilityType.Visible)
			{
				canvasGroup.alpha = 0f;
			}

			SubscribeListeners();
		}

		private void OnHealthChange(IEventData eventData)
		{
			EntityHealthChangeEvent healthEvent = eventData as EntityHealthChangeEvent;
			RefreshBar(0, healthEvent.Health.MaxHealth, healthEvent.CurrentHealth);
		}

		private void OnVisibilityChange(IEventData eventData)
		{
			EntityVisibilityChangeEvent visibilityEvent = eventData as EntityVisibilityChangeEvent;

			if (visibilityEvent.Visibility == VisibilityType.Visible)
			{
				DOTween.Kill(canvasGroup);
				canvasGroup.DOFade(1, 0.3f);
			}
			else if (visibilityEvent.PreviousVisibility == VisibilityType.Visible)
			{
				DOTween.Kill(canvasGroup);
				canvasGroup.DOFade(0, 0.3f);
			}
		}

		private void SubscribeListeners()
		{
			health.Entity.Events.AddListener(OnHealthChange, EntityHealthChangeEvent.EVENT_TYPE);
			health.Entity.Events.AddListener(OnVisibilityChange, EntityVisibilityChangeEvent.EVENT_TYPE);
		}

		private void RefreshBar(float minValue, float maxValue, float currentValue)
		{
			slider.maxValue = maxValue;
			slider.minValue = minValue;
			slider.value = currentValue;
		}
	}
}
