using InstaDungeon.Components;

namespace InstaDungeon.Events
{
	public class EntityDieEvent : BaseEventData
	{
		public static readonly uint EVENT_TYPE = 0X806CF135;

		public override uint EventType { get { return EVENT_TYPE; } }
		public override string Name { get { return "Entity Die Event"; } }
		public Entity Entity { get; private set; }
		public Health Health { get; private set; }

		public EntityDieEvent(Entity entity, Health health)
		{
			Entity = entity;
			Health = health;
		}

		public override BaseEventData CopySpecificData()
		{
			return new EntityDieEvent(Entity, Health);
		}
	}
}
