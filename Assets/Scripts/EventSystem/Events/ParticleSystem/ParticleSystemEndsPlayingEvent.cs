using InstaDungeon.Components;

namespace InstaDungeon.Events
{
	public class ParticleSystemEndsPlayingEvent : BaseEventData
	{
		public static readonly uint EVENT_TYPE = 0XAAEFBD46;

		public override uint EventType { get { return EVENT_TYPE; } }
		public override string Name { get { return "Particle System Ends, Playing Event"; } }
		public ParticleSystemController ParticleSystemController { get; private set; }

		public ParticleSystemEndsPlayingEvent(ParticleSystemController particleSystemController)
		{
			ParticleSystemController = particleSystemController;
		}

		public override BaseEventData CopySpecificData()
		{
			return new ParticleSystemEndsPlayingEvent(ParticleSystemController);
		}
	}
}
