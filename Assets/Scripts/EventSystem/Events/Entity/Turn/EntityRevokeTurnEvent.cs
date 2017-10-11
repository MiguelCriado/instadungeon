using InstaDungeon.Components;

namespace InstaDungeon.Events
{
	public class EntityRevokeTurnEvent : BaseEventData
	{
		public static readonly uint EVENT_TYPE = 0X043F81AA;

		public override uint EventType { get { return EVENT_TYPE; } }
		public override string Name { get { return "Turn Component Revoke Turn Event"; } }

		public Entity Entity { get; private set; }

		public EntityRevokeTurnEvent(Entity entity)
		{
			Entity = entity;
		}

		public override BaseEventData CopySpecificData()
		{
			return new EntityRevokeTurnEvent(Entity);
		}
	}
}
