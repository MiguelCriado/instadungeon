using InstaDungeon.MapGeneration;
using UnityEngine;

namespace InstaDungeon.Settings
{
	[CreateAssetMenu(menuName = "InstaDungeon/Settings/Generation/Layout/Hilbert", fileName = "HilbertLayoutDefinition", order = 1000)]
	public class HilbertLayoutGeneratorDefinition : ScriptableObject
	{
		public HilbertLayoutGeneratorSettings Settings { get { return settings; } }

		[SerializeField] private HilbertLayoutGeneratorSettings settings;
	}
}
