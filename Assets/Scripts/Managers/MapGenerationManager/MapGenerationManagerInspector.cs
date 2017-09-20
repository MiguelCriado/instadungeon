using UnityEngine;

namespace InstaDungeon
{
	public class MapGenerationManagerInspector : MonoBehaviour
	{
		public bool CustomSeed { get { return customSeed; } set { customSeed = value; } }
		public int LevelSeed { get { return levelSeed; } set { levelSeed = value; } }

		[SerializeField] private bool customSeed;
		[SerializeField] private int levelSeed;
	}
}
