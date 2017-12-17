namespace InstaDungeon.Events
{
	public class LevelFinishedEvent : BaseEventData
	{
		public static readonly uint EVENT_TYPE = 0X4201F430;

		public override uint EventType { get { return EVENT_TYPE; } }
		public override string Name { get { return "Level Finished Event"; } }

		public TileMap<Cell> Map { get; private set; }
		public int Level { get; private set; }

		public LevelFinishedEvent(TileMap<Cell> map, int level)
		{
			Map = map;
			Level = level;
		}

		public override BaseEventData CopySpecificData()
		{
			return new LevelFinishedEvent(Map, Level);
		}
	}
}
