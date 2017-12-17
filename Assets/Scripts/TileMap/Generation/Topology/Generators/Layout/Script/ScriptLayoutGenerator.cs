using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Serialization.Json;
using System.Collections.Generic;

namespace InstaDungeon.MapGeneration
{
	public class ScriptLayoutGenerator : BaseLayoutGenerator<ScriptLayoutGeneratorSettings, ScriptLayoutLevelSettings>
	{
		private Script script;

		public ScriptLayoutGenerator(Script script, ScriptLayoutGeneratorSettings settings) : base(settings)
		{
			this.script = script;
		}

		public ScriptLayoutGenerator(Script script, string settings) : base (GenerateSettings(script, settings))
		{
			this.script = script;
		}

		private static ScriptLayoutGeneratorSettings GenerateSettings(Script script, string settings)
		{
			Table settingsTable = JsonTableConverter.JsonToTable(settings, script);

			Table fallbackTable = settingsTable.Get(DynValue.NewString("fallback")).Table;
			ScriptLayoutLevelSettings fallback = new ScriptLayoutLevelSettings(fallbackTable);

			Table entriesTable = settingsTable.Get(DynValue.NewString("entries")).Table;
			List<ScriptLayoutLevelSettings> entryList = new List<ScriptLayoutLevelSettings>();

			foreach (var entry in entriesTable.Values)
			{
				entryList.Add(new ScriptLayoutLevelSettings(entry.Table));
			}

			return new ScriptLayoutGeneratorSettings(fallback, entryList);
		}

		public override Layout NewLayout(int level)
		{
			DynValue result = script.Call(script.Globals["initialize_layout"], GetLevelSettings(level));
			return result.ToObject<Layout>();
		}

		public override bool IsDone(Layout layout, int level)
		{
			DynValue result = script.Call(script.Globals["is_done"], layout, GetLevelSettings(level));
			return result.Boolean;
		}

		public override Layout Iterate(Layout layout, int level)
		{
			DynValue result = script.Call(script.Globals["iterate"], layout, GetLevelSettings(level));
			return result.ToObject<Layout>();
		}

		public void SetRandomSeed(int seed)
		{
			script.Call(script.Globals["set_random_seed"], DynValue.NewNumber(seed));
		}

		private DynValue GetLevelSettings(int level)
		{
			ScriptLayoutLevelSettings levelSettings = settings.GetSettings(level);
			return DynValue.NewTable(levelSettings.Settings);
		}
	}
}
