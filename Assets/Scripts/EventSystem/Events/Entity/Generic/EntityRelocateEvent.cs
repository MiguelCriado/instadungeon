namespace InstaDungeon.Events
{
	public class EntityRelocateEvent : BaseEventData
	{
		public static readonly uint EVENT_TYPE = 0x83c25a35;

		public override uint EventType { get { return EVENT_TYPE; } }
		public override string Name { get { return "EntityRelocateEvent"; } }
		public uint EntityId { get; private set; }
		public int2 LastPosition { get; private set; }
		public int2 CurrentPosition { get; private set; }

		public EntityRelocateEvent(uint entityId, int2 lastPosition, int2 currentPosition)
		{
			EntityId = entityId;
			LastPosition = lastPosition;
			CurrentPosition = currentPosition;
		}

		public override BaseEventData CopySpecificData()
		{
			return new EntityRelocateEvent(EntityId, LastPosition, CurrentPosition);
		}
	}
}
