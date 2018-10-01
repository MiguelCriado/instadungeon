using InstaDungeon.Events;
using UnityEngine;

namespace InstaDungeon.Components
{
	[RequireComponent(typeof(ParticleSystem))]
	public class ParticleSystemController : MonoBehaviour
	{
		public EventSystem Events { get { return events; } }
		public ParticleSystem ParticleSystem { get { return attachedSystem; } }

		[SerializeField] bool playOnEnable;

		private EventSystem events;
		private ParticleSystem attachedSystem;

		private void Reset()
		{
			playOnEnable = true;
		}

		private void Awake()
		{
			events = new EventSystem();
			attachedSystem = GetComponent<ParticleSystem>();
		}

		private void OnEnable()
		{
			if (playOnEnable)
			{
				attachedSystem.Play();
			}
		}

		private void Update()
		{
			if (!attachedSystem.IsAlive())
			{
				events.TriggerEvent(new ParticleSystemEndsPlayingEvent(this));
			}
		}
	}
}
