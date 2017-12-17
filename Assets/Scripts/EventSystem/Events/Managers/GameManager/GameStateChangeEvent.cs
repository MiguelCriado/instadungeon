namespace InstaDungeon.Events
{
	public class GameStateChangeEvent : BaseEventData
	{
		public static readonly uint EVENT_TYPE = 0XD32DEFA9;

		public override uint EventType { get { return EVENT_TYPE; } }
		public override string Name { get { return "Game State Change Event"; } }

		public GameState LastState { get; private set; }
		public GameState State { get; private set; }

		public GameStateChangeEvent(GameState lastState, GameState state)
		{
			LastState = lastState;
			State = state;
		}

		public override BaseEventData CopySpecificData()
		{
			return new GameStateChangeEvent(LastState, State);
		}
	}
}
