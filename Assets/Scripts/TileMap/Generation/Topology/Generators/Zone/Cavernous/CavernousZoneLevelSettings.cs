using UnityEngine;

namespace InstaDungeon.MapGeneration
{
	[System.Serializable]
	public class CavernousZoneLevelSettings : IZoneLevelSettings
	{
		public int MinLevel { get { return minLevel; } }
		public float InitialWallProbability { get { return initialWallProbability; } }
		public int Iterations { get { return iterations; } }
		public int RefiningIterations { get { return refiningIterations; } }

		[SerializeField] private int minLevel;
		[SerializeField] private float initialWallProbability;
		[SerializeField] private int iterations;
		[SerializeField] private int refiningIterations;

		public CavernousZoneLevelSettings(int minLevel, float initialWallProbability, int iterations, int refiningIterations)
		{
			this.minLevel = minLevel;
			this.initialWallProbability = initialWallProbability;
			this.iterations = iterations;
			this.refiningIterations = refiningIterations;
		}
	}
}
