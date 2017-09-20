using InstaDungeon.MapGeneration;
using InstaDungeon.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InstaDungeon
{
	public class MapGenerationManager : Manager
	{
		private static readonly string DefaultLayoutGeneratorPath = "Settings/DefaultHilbertLayoutDefinition";
		private static readonly string DefaultZoneGeneratorPath = "Settings/DefaultCavernousZoneDefinition";

		public MapGenerator Generator { get; private set; }

		private MapGenerationManagerInspector settings;

		public MapGenerationManager()
		{
			Generator = GetDefaultGenerator();
			settings = gameObject.AddComponent<MapGenerationManagerInspector>();
		}

		public void SetLayoutGenerator(ILayoutGenerator layoutGenerator)
		{
			Generator.LayoutGenerator = layoutGenerator;
		}

		public void SetZoneGenerator(IZoneGenerator zoneGenerator)
		{
			Generator.ZoneGenerator = zoneGenerator;
		}

		public TileMap<Cell> GenerateNewMap(int level, int levelSeed)
		{
			return Generator.GenerateNewMap(level, levelSeed);
		}

		public TileMap<Cell> GenerateNewMap(int level)
		{
			if (settings.CustomSeed == false)
			{
				settings.LevelSeed = System.Guid.NewGuid().GetHashCode() ^ System.DateTime.UtcNow.Millisecond;
			}

			return GenerateNewMap(level, settings.LevelSeed);
		}

		protected override void OnSceneUnLoaded(Scene scene)
		{
			base.OnSceneUnLoaded(scene);
			settings = gameObject.AddComponent<MapGenerationManagerInspector>();
		}

		private MapGenerator GetDefaultGenerator()
		{
			MapGenerator result = null;

			HilbertLayoutGeneratorDefinition layoutGeneratorDefinition = Resources.Load<HilbertLayoutGeneratorDefinition>(DefaultLayoutGeneratorPath);
			HilbertLayoutGenerator layoutGenerator = new HilbertLayoutGenerator(layoutGeneratorDefinition.Settings);

			CavernousZoneGeneratorDefinition zoneGeneratorDefinition = Resources.Load<CavernousZoneGeneratorDefinition>(DefaultZoneGeneratorPath);
			CavernousZoneGenerator zoneGenerator = new CavernousZoneGenerator(zoneGeneratorDefinition.Settings);

			result = new MapGenerator(zoneGenerator, layoutGenerator);

			return result;
		}
	}
}
