using InstaDungeon.MapGeneration;

namespace InstaDungeon
{
	public enum ControlMode
	{
		Manual,
		Auto
	}

	public class GameSettings
	{
		public ILayoutGenerator LayoutGenerator;
		public IZoneGenerator ZoneGenerator;
		public int Seed;
		public int NumLevels;
		public bool GenerateStatsLog;
		public ControlMode ControlMode;

		public GameSettings(ILayoutGenerator layoutGenerator, IZoneGenerator zoneGenerator, int seed, int numLevels, bool generateStatsLog, ControlMode controlMode)
		{
			LayoutGenerator = layoutGenerator;
			ZoneGenerator = zoneGenerator;
			Seed = seed;
			NumLevels = numLevels;
			GenerateStatsLog = generateStatsLog;
			ControlMode = controlMode;
		}
	}
}
