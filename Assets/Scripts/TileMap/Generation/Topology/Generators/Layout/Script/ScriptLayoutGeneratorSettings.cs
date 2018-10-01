using System.Collections.Generic;

namespace InstaDungeon.MapGeneration
{
	public class ScriptLayoutGeneratorSettings : BaseLayoutGeneratorSettings<ScriptLayoutLevelSettings>
	{
		public ScriptLayoutGeneratorSettings(ScriptLayoutLevelSettings fallbackSettings, List<ScriptLayoutLevelSettings> settings) : base(fallbackSettings, settings)
		{

		}
	}
}
