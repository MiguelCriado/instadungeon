using InstaDungeon.Components;

namespace InstaDungeon.Events
{
	public class EntityHealthChangeEvent : BaseEventData
	{
		public static readonly uint EVENT_TYPE = 0X6670A49F;

		public override uint EventType { get { return EVENT_TYPE; } }
		public override string Name { get { return "Entity Health Change Event"; } }

		public Entity Entity { get; private set; }
		public Health Health { get; private set; }
		public int PreviousHealth { get; private set; }
		public int CurrentHealth { get; private set; }

		public EntityHealthChangeEvent(Entity entity, Health health, int previousHealth, int currentHealth)
		{
			Entity = entity;
			Health = health;
			PreviousHealth = previousHealth;
			CurrentHealth = currentHealth;
		}

		public override BaseEventData CopySpecificData()
		{
			return new EntityHealthChangeEvent(Entity, Health, PreviousHealth, CurrentHealth);
		}
	}
}
