using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Serialization.Json;
using System.Collections.Generic;

namespace InstaDungeon.MapGeneration
{
	public class ScriptZoneGenerator : BaseZoneGenerator<ScriptZoneGeneratorSettings, ScriptZoneLevelSettings>
	{
		private Script script;
		private string name;

		public ScriptZoneGenerator(string name, Script script, ScriptZoneGeneratorSettings settings) : base(settings)
		{
			this.script = script;
			this.name = name;
		}

		public ScriptZoneGenerator(string name, Script script, string settings) : base(GenerateSettings(script, settings))
		{
			this.script = script;
			this.name = name;
		}

		private static ScriptZoneGeneratorSettings GenerateSettings(Script script, string settings)
		{
			Table settingsTable = JsonTableConverter.JsonToTable(settings, script);

			Table fallbackTable = settingsTable.Get(DynValue.NewString("fallback")).Table;
			ScriptZoneLevelSettings fallback = new ScriptZoneLevelSettings(fallbackTable);

			Table entriesTable = settingsTable.Get(DynValue.NewString("entries")).Table;
			List<ScriptZoneLevelSettings> entryList = new List<ScriptZoneLevelSettings>();

			foreach (var entry in entriesTable.Values)
			{
				entryList.Add(new ScriptZoneLevelSettings(entry.Table));
			}

			return new ScriptZoneGeneratorSettings(fallback, entryList);
		}

		public override TileMap<TileType> PreConnectZones(TileMap<TileType> map, int level)
		{
			DynValue result = script.Call(script.Globals["pre_connect_zones"], map, GetLevelSettings(level));
			return result.ToObject<TileMap<TileType>>();
		}

		public override TileMap<TileType> Generate(TileMap<TileType> map, int level)
		{
			DynValue result = script.Call(script.Globals["generate"], map, GetLevelSettings(level));
			return result.ToObject<TileMap<TileType>>();
		}

		public override TileMap<TileType> PostConnectZones(TileMap<TileType> map, int level)
		{
			DynValue result = script.Call(script.Globals["post_connect_zones"], map, GetLevelSettings(level));
			return result.ToObject<TileMap<TileType>>();
		}

		public override int2 GetSpawnPoint(TileMap<TileType> map, int level)
		{
			DynValue result = script.Call(script.Globals["get_spawn_point"], map, GetLevelSettings(level));
			return result.ToObject<int2>();
		}

		public override int2 GetExitPoint(TileMap<TileType> map, int level)
		{
			DynValue result = script.Call(script.Globals["get_exit_point"], map, GetLevelSettings(level));
			return result.ToObject<int2>();
		}

		public void SetRandomSeed(int seed)
		{
			script.Call(script.Globals["set_random_seed"], DynValue.NewNumber(seed));
		}

		public override string ToString()
		{
			return name;
		}

		private DynValue GetLevelSettings(int level)
		{
			ScriptZoneLevelSettings levelSettings = settings.GetSettings(level);
			return DynValue.NewTable(levelSettings.Settings);
		}
	}
}
