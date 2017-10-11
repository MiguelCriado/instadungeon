using InstaDungeon.Components;

namespace InstaDungeon.Events
{
	public class EntityTurnDoneEvent : BaseEventData
	{
		public static readonly uint EVENT_TYPE = 0X0C550964;

		public override uint EventType { get { return EVENT_TYPE; } }
		public override string Name { get { return "Turn Component Turn Done Event"; } }

		public Entity Entity { get; private set; }

		public EntityTurnDoneEvent(Entity entity)
		{
			Entity = entity;
		}

		public override BaseEventData CopySpecificData()
		{
			return new EntityTurnDoneEvent(Entity);
		}
	}
}
