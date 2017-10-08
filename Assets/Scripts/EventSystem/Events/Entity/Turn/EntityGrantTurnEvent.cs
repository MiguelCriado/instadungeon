using InstaDungeon.Components;

namespace InstaDungeon.Events
{
	public class EntityGrantTurnEvent : BaseEventData
	{
		public static readonly uint EVENT_TYPE = 0X981E6639;

		public override uint EventType { get { return EVENT_TYPE; } }
		public override string Name { get { return "Turn Component Grant Turn Event"; } }

		public Entity Entity { get; private set; }

		public EntityGrantTurnEvent(Entity entity)
		{
			Entity = entity;
		}

		public override BaseEventData CopySpecificData()
		{
			return new EntityGrantTurnEvent(Entity);
		}
	}
}
