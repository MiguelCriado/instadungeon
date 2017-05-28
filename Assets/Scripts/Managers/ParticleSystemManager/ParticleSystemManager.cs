using InstaDungeon.Components;
using InstaDungeon.Events;
using System.Collections.Generic;
using UnityEngine;

namespace InstaDungeon
{
	public class ParticleSystemManager : Manager
	{
		private HashSet<ParticleSystemController> aliveSystems;
		private PrefabLoader<ParticleSystemController> loader;
		private Transform particlesContainer;

		public ParticleSystemManager() : base()
		{
			aliveSystems = new HashSet<ParticleSystemController>();
			loader = new PrefabLoader<ParticleSystemController>("Particle FX");
			particlesContainer = GetSceneContainer("World", "Particle Systems");
		}

		public ParticleSystemController Spawn(string particleSystemName, Vector3 spawnPosition)
		{
			ParticleSystemController result = loader.Spawn(particleSystemName, particlesContainer);

			if (result != null)
			{
				result.transform.position = spawnPosition;
				aliveSystems.Add(result);
				SubscribeEvents(result);
			}

			return result;
		}

		public ParticleSystemController Spawn(string particleSystemName)
		{
			return Spawn(particleSystemName, Vector3.zero);
		}

		#region [Events Reaction]

		private void OnParticleEndsPlaying(IEventData eventData)
		{
			ParticleSystemEndsPlayingEvent particleSystemEvent = eventData as ParticleSystemEndsPlayingEvent;
			UnsubscribeEvents(particleSystemEvent.ParticleSystemController);

			loader.Dispose(particleSystemEvent.ParticleSystemController);
		}

		#endregion

		private void SubscribeEvents(ParticleSystemController controller)
		{
			controller.Events.AddListener(OnParticleEndsPlaying, ParticleSystemEndsPlayingEvent.EVENT_TYPE);
		}

		private void UnsubscribeEvents(ParticleSystemController controller)
		{
			controller.Events.RemoveListener(OnParticleEndsPlaying, ParticleSystemEndsPlayingEvent.EVENT_TYPE);
		}
	}
}
