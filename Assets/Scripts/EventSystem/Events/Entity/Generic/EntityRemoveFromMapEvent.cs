using InstaDungeon.Components;

namespace InstaDungeon.Events
{
	public class EntityRemoveFromMapEvent : BaseEventData
	{
		public static readonly uint EVENT_TYPE = 0X5CC434FB;

		public override uint EventType { get { return EVENT_TYPE; } }
		public override string Name { get { return "Entity Remove From Map Event"; } }

		public Entity Entity { get; private set; }
		public int2 Position { get; private set; }

		public EntityRemoveFromMapEvent(Entity entity, int2 position)
		{
			Entity = entity;
			Position = position;
		}

		public override BaseEventData CopySpecificData()
		{
			return new EntityRemoveFromMapEvent(Entity, Position);
		}
	}
}
