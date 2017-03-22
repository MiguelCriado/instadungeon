namespace InstaDungeon.Events
{
	public class EntityFinishMovementEvent : BaseEventData
	{
		public static readonly uint EVENT_TYPE = 0x9089fa66;

		public override uint EventType { get { return EVENT_TYPE; } }
		public override string Name { get { return "EntityFinishMovementEvent"; } }
		public uint EntityId { get; private set; }
		public int2 PreviousPosition { get; private set; }
		public int2 CurrentPosition { get; private set; }

		public EntityFinishMovementEvent(uint entityId, int2 previousPosition, int2 currentPosition)
		{
			EntityId = entityId;
			PreviousPosition = previousPosition;
			CurrentPosition = currentPosition;
		}

		public override IEventData Copy()
		{
			EntityFinishMovementEvent result = new EntityFinishMovementEvent(EntityId, PreviousPosition, CurrentPosition);
			result.TimeStamp = TimeStamp;

			return result;
		}
	}
}
