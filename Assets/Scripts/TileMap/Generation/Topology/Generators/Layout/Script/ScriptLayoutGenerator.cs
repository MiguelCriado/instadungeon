using MoonSharp.Interpreter;

namespace InstaDungeon.MapGeneration
{
	public class ScriptLayoutGenerator : BaseLayoutGenerator<ScriptLayoutGeneratorSettings, ScriptLayoutLevelSettings>
	{
		private Script script;

		public ScriptLayoutGenerator(Script script, ScriptLayoutGeneratorSettings settings) : base(settings)
		{
			this.script = script;
		}

		public override Layout NewLayout(int level)
		{
			Layout layout = new Layout();
			DynValue result = script.Call(script.Globals["new_layout"], layout, level);
			return result.ToObject<Layout>();
		}

		public override bool IsDone()
		{
			DynValue result = script.Call(script.Globals["is_done"]);
			return result.Boolean;
		}

		public override Layout Iterate(Layout layout, int level)
		{
			DynValue result = script.Call(script.Globals["iterate"], layout, level);
			return result.ToObject<Layout>();
		}
	}
}
