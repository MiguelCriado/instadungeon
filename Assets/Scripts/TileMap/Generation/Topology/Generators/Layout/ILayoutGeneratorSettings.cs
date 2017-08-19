namespace InstaDungeon.MapGeneration
{
	public interface ILayoutGeneratorSettings<T> where T : ILayoutLevelSettings
	{
		T GetSettings(int level);
	}
}
