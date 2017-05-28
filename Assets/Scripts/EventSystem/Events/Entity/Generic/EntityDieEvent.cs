using InstaDungeon.Components;

namespace InstaDungeon.Events
{
	public class EntityDieEvent : BaseEventData
	{
		public static readonly uint EVENT_TYPE = 0X806CF135;

		public override uint EventType { get { return EVENT_TYPE; } }
		public override string Name { get { return "Entity Die Event"; } }
		public Health Health { get; private set; }

		public EntityDieEvent(Health health)
		{
			Health = health;
		}

		public override IEventData Copy()
		{
			EntityDieEvent result = new EntityDieEvent(Health);
			result.TimeStamp = TimeStamp;

			return result;
		}
	}
}
