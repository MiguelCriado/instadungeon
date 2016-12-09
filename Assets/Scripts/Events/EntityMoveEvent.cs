namespace InstaDungeon.Events
{
	public class EntityMoveEvent : BaseEventData
	{
		public static readonly uint EVENT_TYPE = 0x73cd2e33;

		public override uint EventType { get { return EVENT_TYPE; } }
		public override string Name { get { return "EntityMoveEvent"; } }
		public uint EntityId { get; private set; }
		public int2 PreviousPosition { get; private set; }
		public int2 CurrentPosition { get; private set; }

		public EntityMoveEvent(uint entityId, int2 previousPosition, int2 currentPosition)
		{
			EntityId = entityId;
			PreviousPosition = previousPosition;
			CurrentPosition = currentPosition;
		}

		public override IEventData Copy()
		{
			EntityMoveEvent result = new EntityMoveEvent(EntityId, PreviousPosition, CurrentPosition);
			result.TimeStamp = TimeStamp;

			return result;
		}
	}
}
