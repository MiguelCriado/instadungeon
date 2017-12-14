namespace InstaDungeon.MapGeneration
{
	public abstract class BaseLayoutGenerator<T1, T2> : ILayoutGenerator<T1, T2>
	where T1 : ILayoutGeneratorSettings<T2>
	where T2 : ILayoutLevelSettings
	{
		protected T1 settings;

		public BaseLayoutGenerator(T1 settings)
		{
			SetSettings(settings);
		}

		public void SetSettings(T1 settings)
		{
			this.settings = settings;
		}

		public abstract Layout NewLayout(int level);

		public abstract Layout Iterate(Layout layout, int level);

		public abstract bool IsDone(Layout layout, int level);
	}
}
