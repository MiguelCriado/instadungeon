namespace InstaDungeon.Events
{
	public class NewGameStartsEvent : BaseEventData
	{
		public static readonly uint EVENT_TYPE = 0XF4D6C1B7;

		public override uint EventType { get { return EVENT_TYPE; } }
		public override string Name { get { return "New Game Starts Event"; } }


		public override BaseEventData CopySpecificData()
		{
			return new NewGameStartsEvent();
		}
	}
}
