using InstaDungeon.Components;

namespace InstaDungeon.Events
{
	public class ActorRemovedFromMapEvent : BaseEventData
	{
		public static readonly uint EVENT_TYPE = 0X149DDC27;

		public override uint EventType { get { return EVENT_TYPE; } }
		public override string Name { get { return "Actor Removed From Map Event"; } }

		public Entity Actor { get; private set; }
		public int2 Position { get; private set; }

		public ActorRemovedFromMapEvent(Entity actor, int2 position)
		{
			Actor = actor;
			Position = position;
		}

		public override BaseEventData CopySpecificData()
		{
			return new ActorRemovedFromMapEvent(Actor, Position);
		}
	}
}
