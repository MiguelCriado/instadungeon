using System.Collections.Generic;

namespace InstaDungeon.MapGeneration
{
	public class ScriptZoneGeneratorSettings : BaseZoneGeneratorSettings<ScriptZoneLevelSettings>
	{
		public ScriptZoneGeneratorSettings(ScriptZoneLevelSettings fallbackSettings, List<ScriptZoneLevelSettings> settings) : base(fallbackSettings, settings)
		{

		}
	}
}
