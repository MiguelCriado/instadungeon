namespace InstaDungeon.MapGeneration
{
	public interface IZoneGeneratorSettings<T> where T : IZoneLevelSettings
	{
		T GetSettings(int level);
	}
}
