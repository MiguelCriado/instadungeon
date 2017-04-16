using InstaDungeon.Components;

namespace InstaDungeon.Events
{
	public class DoorOpenEvent : BaseEventData
	{
		public static readonly uint EVENT_TYPE = 0x7744aaba;

		public override uint EventType { get { return EVENT_TYPE; } }
		public override string Name { get { return "DoorOpenEvent"; } }
		public Entity Door { get; private set; }

		public DoorOpenEvent(Entity door)
		{
			Door = door;
		}

		public override IEventData Copy()
		{
			DoorOpenEvent result = new DoorOpenEvent(Door);
			result.TimeStamp = TimeStamp;

			return result;
		}
	}
}
