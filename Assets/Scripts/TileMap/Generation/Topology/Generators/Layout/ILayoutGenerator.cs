namespace InstaDungeon.MapGeneration
{
	public interface ILayoutGenerator
	{
		Layout NewLayout(int level);
		Layout Iterate(Layout layout, int level);
		bool IsDone();
	}

	public interface ILayoutGenerator<T1, T2> : ILayoutGenerator
	where T1 : ILayoutGeneratorSettings<T2>
	where T2 : ILayoutLevelSettings
	{
		void SetSettings(T1 settings);
	}
}
