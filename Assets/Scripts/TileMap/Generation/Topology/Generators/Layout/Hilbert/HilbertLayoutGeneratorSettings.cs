using System.Collections.Generic;

namespace InstaDungeon.MapGeneration
{
	[System.Serializable]
	public class HilbertLayoutGeneratorSettings : BaseLayoutGeneratorSettings<HilbertLayoutLevelSettings>
	{
		public HilbertLayoutGeneratorSettings(List<HilbertLayoutLevelSettings> settings) : base(settings)
		{

		}
	}
}
