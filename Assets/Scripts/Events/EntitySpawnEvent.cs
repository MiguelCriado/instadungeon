namespace InstaDungeon.Events
{
	public class EntitySpawnEvent : BaseEventData
	{
		public static readonly uint EVENT_TYPE = 0xbe625271;

		public override uint EventType { get { return EVENT_TYPE; } }
		public override string Name { get { return "EntitySpawnEvent"; } }
		public uint EntityId { get; private set; }

		public EntitySpawnEvent(uint entityId)
		{
			EntityId = entityId;
		}

		public override IEventData Copy()
		{
			EntitySpawnEvent result = new EntitySpawnEvent(EntityId);
			result.TimeStamp = TimeStamp;

			return result;
		}
	}
}
