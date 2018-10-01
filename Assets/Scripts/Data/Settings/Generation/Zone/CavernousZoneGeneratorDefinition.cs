using InstaDungeon.MapGeneration;
using UnityEngine;

namespace InstaDungeon.Settings
{
	[CreateAssetMenu(menuName = "InstaDungeon/Settings/Generation/Zone/Cavernous", fileName = "CavernousZoneDefinition", order = 1000)]
	public class CavernousZoneGeneratorDefinition : ScriptableObject
	{
		public CavernousZoneGeneratorSettings Settings { get { return settings; } }

		[SerializeField] private CavernousZoneGeneratorSettings settings;
	}
}
