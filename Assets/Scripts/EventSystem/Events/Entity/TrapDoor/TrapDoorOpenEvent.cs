using InstaDungeon.Components;

namespace InstaDungeon.Events
{
	public class TrapDoorOpenEvent : BaseEventData
	{
		public static readonly uint EVENT_TYPE = 0XE8EFA744;

		public override uint EventType { get { return EVENT_TYPE; } }
		public override string Name { get { return "Trap Door Open Event"; } }

		public Entity TrapDoor { get; private set; }

		public TrapDoorOpenEvent(Entity trapDoor)
		{
			TrapDoor = trapDoor;
		}

		public override BaseEventData CopySpecificData()
		{
			return new TrapDoorOpenEvent(TrapDoor);
		}
	}
}
