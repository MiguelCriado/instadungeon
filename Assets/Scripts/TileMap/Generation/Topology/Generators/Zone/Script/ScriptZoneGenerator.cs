using MoonSharp.Interpreter;

namespace InstaDungeon.MapGeneration
{
	public class ScriptZoneGenerator : BaseZoneGenerator<ScriptZoneGeneratorSettings, ScriptZoneLevelSettings>
	{
		private Script script;

		public ScriptZoneGenerator(Script script, ScriptZoneGeneratorSettings settings) : base(settings)
		{
			this.script = script;
		}

		public override TileMap<TileType> PreConnectZones(TileMap<TileType> map, int level)
		{
			DynValue result = script.Call(script.Globals["pre_connect_zones"], map, level);
			return result.ToObject<TileMap<TileType>>();
		}

		public override TileMap<TileType> Generate(TileMap<TileType> map, int level)
		{
			DynValue result = script.Call(script.Globals["generate"], map, level);
			return result.ToObject<TileMap<TileType>>();
		}

		public override TileMap<TileType> PostConnectZones(TileMap<TileType> map, int level)
		{
			DynValue result = script.Call(script.Globals["post_connect_zones"], map, level);
			return result.ToObject<TileMap<TileType>>();
		}

		public override int2 GetSpawnPoint(TileMap<TileType> map, int level)
		{
			DynValue result = script.Call(script.Globals["get_spawn_point"], map, level);
			return result.ToObject<int2>();
		}

		public override int2 GetExitPoint(TileMap<TileType> map, int level)
		{
			DynValue result = script.Call(script.Globals["get_exit_point"], map, level);
			return result.ToObject<int2>();
		}
	}
}
