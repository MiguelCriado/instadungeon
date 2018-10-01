using InstaDungeon.Events;
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

		public EventSystem Events { get; private set; }
		public MapGenerator Generator { get; private set; }

		private MapGenerationManagerInspector settings;
		private Random.State savedState;

		public MapGenerationManager()
		{
			Events = new EventSystem();
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
			TileMap<Cell> result = Generator.GenerateNewMap(level, levelSeed);
			Events.TriggerEvent(new MapGenerationNewMapEvent(result, Generator, level, levelSeed));
			return result;
		}

		public TileMap<Cell> GenerateNewMap(int level)
		{
			if (settings.CustomSeed == false)
			{
				settings.LevelSeed = System.Guid.NewGuid().GetHashCode() ^ System.DateTime.UtcNow.Millisecond;
			}

			return GenerateNewMap(level, settings.LevelSeed);
		}

		protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			base.OnSceneLoaded(scene, mode);
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
