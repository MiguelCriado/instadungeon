using InstaDungeon.Components;

namespace InstaDungeon.Events
{
	public class EntityVisibilityChangeEvent : BaseEventData
	{
		public static readonly uint EVENT_TYPE = 0XDFAF9622;

		public override uint EventType { get { return EVENT_TYPE; } }
		public override string Name { get { return "Entity Visibility Change Event"; } }

		public Entity Entity { get; private set; }
		public VisibilityType PreviousVisibility { get; private set; }
		public VisibilityType Visibility { get; private set; }

		public EntityVisibilityChangeEvent(Entity entity, VisibilityType previousVisibility, VisibilityType visibility)
		{
			Entity = entity;
			PreviousVisibility = previousVisibility;
			Visibility = visibility;
		}

		public override BaseEventData CopySpecificData()
		{
			return new EntityVisibilityChangeEvent(Entity, PreviousVisibility, Visibility);
		}
	}
}
