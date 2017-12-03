using System.Collections.Generic;

namespace InstaDungeon.MapGeneration
{
	[System.Serializable]
	public class HilbertLayoutGeneratorSettings : BaseLayoutGeneratorSettings<HilbertLayoutLevelSettings>
	{
		public HilbertLayoutGeneratorSettings(HilbertLayoutLevelSettings fallbackSettings, List<HilbertLayoutLevelSettings> settings) : base(fallbackSettings, settings)
		{

		}
	}
}
