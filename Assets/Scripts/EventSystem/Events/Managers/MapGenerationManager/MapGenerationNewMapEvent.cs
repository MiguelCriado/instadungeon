using InstaDungeon.MapGeneration;

namespace InstaDungeon.Events
{
	public class MapGenerationNewMapEvent : BaseEventData
	{
		public static readonly uint EVENT_TYPE = 0X577EC51E;

		public override uint EventType { get { return EVENT_TYPE; } }
		public override string Name { get { return "Map Generation New Map Event"; } }

		public TileMap<Cell> Map { get; private set; }
		public MapGenerator Generator { get; private set; }
		public int Level { get; private set; }
		public int LevelSeed { get; private set; }

		public MapGenerationNewMapEvent(TileMap<Cell> map, MapGenerator generator, int level, int levelSeed)
		{
			Map = map;
			Generator = generator;
			Level = level;
			LevelSeed = levelSeed; 
		}

		public override BaseEventData CopySpecificData()
		{
			return new MapGenerationNewMapEvent(Map, Generator, Level, LevelSeed);
		}
	}
}
