using System.Collections.Generic;

namespace InstaDungeon.MapGeneration
{
	[System.Serializable]
	public class CavernousZoneGeneratorSettings : BaseZoneGeneratorSettings<CavernousZoneLevelSettings>
	{
		public CavernousZoneGeneratorSettings(List<CavernousZoneLevelSettings> settings) : base(settings)
		{

		}
	}
}
