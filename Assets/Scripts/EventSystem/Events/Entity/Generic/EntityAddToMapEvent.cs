using InstaDungeon.Components;

namespace InstaDungeon.Events
{
	public class EntityAddToMapEvent : BaseEventData
	{
		public static readonly uint EVENT_TYPE = 0x84af6889;

		public override uint EventType { get { return EVENT_TYPE; } }
		public override string Name { get { return "EntityAddToMapEvent"; } }
		public Entity Entity { get; private set; }

		public EntityAddToMapEvent(Entity entity)
		{
			Entity = entity;
		}

		public override IEventData Copy()
		{
			EntityAddToMapEvent result = new EntityAddToMapEvent(Entity);
			result.TimeStamp = TimeStamp;

			return result;
		}
	}
}
