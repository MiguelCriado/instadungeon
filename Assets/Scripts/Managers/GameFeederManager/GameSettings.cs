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
		public ControlMode ControlMode;

		public GameSettings(ILayoutGenerator layoutGenerator, IZoneGenerator zoneGenerator, int seed, ControlMode controlMode)
		{
			LayoutGenerator = layoutGenerator;
			ZoneGenerator = zoneGenerator;
			Seed = seed;
			ControlMode = controlMode;
		}
	}
}
