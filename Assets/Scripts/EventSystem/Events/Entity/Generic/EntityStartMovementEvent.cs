namespace InstaDungeon.Events
{
	public class EntityStartMovementEvent : BaseEventData
	{
		public static readonly uint EVENT_TYPE = 0x73cd2e33;

		public override uint EventType { get { return EVENT_TYPE; } }
		public override string Name { get { return "EntityStartMovementEvent"; } }
		public uint EntityId { get; private set; }
		public int2 CurrentPosition { get; private set; }
		public int2 NextPosition { get; private set; }

		public EntityStartMovementEvent(uint entityId, int2 currentPosition, int2 nextPosition)
		{
			EntityId = entityId;
			CurrentPosition = currentPosition;
			NextPosition = nextPosition;
		}

		public override IEventData Copy()
		{
			EntityStartMovementEvent result = new EntityStartMovementEvent(EntityId, CurrentPosition, NextPosition);
			result.TimeStamp = TimeStamp;

			return result;
		}
	}
}
