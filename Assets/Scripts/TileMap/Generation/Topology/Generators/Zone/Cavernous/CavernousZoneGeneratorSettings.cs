using System.Collections.Generic;

namespace InstaDungeon.MapGeneration
{
	[System.Serializable]
	public class CavernousZoneGeneratorSettings : BaseZoneGeneratorSettings<CavernousZoneLevelSettings>
	{
		public CavernousZoneGeneratorSettings(CavernousZoneLevelSettings fallbackSettings, List<CavernousZoneLevelSettings> settings) : base(fallbackSettings, settings)
		{

		}
	}
}
