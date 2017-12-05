using MoonSharp.Interpreter;

namespace InstaDungeon.MapGeneration
{
	public class ScriptZoneLevelSettings : IZoneLevelSettings
	{
		public int MinLevel { get; private set; }
		public Table Settings { get; private set; }

		public ScriptZoneLevelSettings(Table settings)
		{
			MinLevel = (int)settings.Get(DynValue.NewString("min_level")).Number;
			Settings = settings.Get(DynValue.NewString("data")).Table;
		}
	}
}
