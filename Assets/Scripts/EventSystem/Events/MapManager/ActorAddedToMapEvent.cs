using InstaDungeon.Components;

namespace InstaDungeon.Events
{
	public class ActorAddedToMapEvent : BaseEventData
	{
		public static readonly uint EVENT_TYPE = 0X7CE21235;

		public override uint EventType { get { return EVENT_TYPE; } }
		public override string Name { get { return "Actor Added To Map Event"; } }

		public Entity Actor { get; private set; }
		public int2 Position { get; private set; }

		public ActorAddedToMapEvent(Entity actor, int2 position)
		{
			Actor = actor;
			Position = position;
		}

		public override BaseEventData CopySpecificData()
		{
			return new ActorAddedToMapEvent(Actor, Position);
		}
	}
}
