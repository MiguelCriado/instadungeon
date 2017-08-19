using UnityEngine;

namespace InstaDungeon.MapGeneration
{
	[System.Serializable]
	public class HilbertLayoutLevelSettings : ILayoutLevelSettings
	{
		public int MinLevel { get { return minLevel; } }
		public int2 Dimensions { get { return dimensions; } }
		public int2 ZoneDimensions { get { return zoneDimensions; } }

		[SerializeField] private int minLevel;
		[SerializeField] private int2 dimensions;
		[SerializeField] private int2 zoneDimensions;

		public HilbertLayoutLevelSettings(int minLevel, int2 dimensions, int2 zoneDimensions)
		{
			this.minLevel = minLevel;
			this.dimensions = dimensions;
			this.zoneDimensions = zoneDimensions;
		}
	}
}
