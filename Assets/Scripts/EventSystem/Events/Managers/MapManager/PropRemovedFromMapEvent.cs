using InstaDungeon.Components;

namespace InstaDungeon.Events
{
	public class PropRemovedFromMapEvent : BaseEventData
	{
		public static readonly uint EVENT_TYPE = 0X9DCD5487;

		public override uint EventType { get { return EVENT_TYPE; } }
		public override string Name { get { return "Prop Removed From Map Event"; } }

		public Entity Prop { get; private set; }
		public int2 Position { get; private set; }

		public PropRemovedFromMapEvent(Entity actor, int2 position)
		{
			Prop = actor;
			Position = position;
		}

		public override BaseEventData CopySpecificData()
		{
			return new PropRemovedFromMapEvent(Prop, Position);
		}
	}
}
