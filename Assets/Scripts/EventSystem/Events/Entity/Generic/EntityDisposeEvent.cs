using InstaDungeon.Components;

namespace InstaDungeon.Events
{
	public class EntityDisposeEvent : BaseEventData
	{
		public static readonly uint EVENT_TYPE = 0XFE536066;

		public override uint EventType { get { return EVENT_TYPE; } }
		public override string Name { get { return "Entity Dispose Event"; } }
		public Entity Entity { get; private set; }

		public EntityDisposeEvent(Entity entity)
		{
			Entity = entity;
		}

		public override IEventData Copy()
		{
			EntityDisposeEvent result = new EntityDisposeEvent(Entity);
			result.TimeStamp = TimeStamp;

			return result;
		}
	}
}
