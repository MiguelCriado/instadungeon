using Random = UnityEngine.Random;

namespace InstaDungeon.MapGeneration
{
	public static class ZoneConnector
	{
		public static TileMap<TileType> BuildMap(ILayoutGenerator layoutGenerator, IZoneGenerator zoneGenerator, int level, int levelSeed)
		{
			Random.InitState(levelSeed);
			TileMap<TileType> result = new TileMap<TileType>();

			Layout layout = layoutGenerator.NewLayout(level);
			result.Layout = layout;

			while (!layoutGenerator.IsDone())
			{
				result.Layout = layoutGenerator.Iterate(layout, level);

				result = zoneGenerator.PreConnectZones(result, level);
				result = zoneGenerator.Generate(result, level);
				result = zoneGenerator.PostConnectZones(result, level);
			}

			result.SpawnPoint = zoneGenerator.GetSpawnPoint(result, level);
			result.ExitPoint = zoneGenerator.GetExitPoint(result, level);

			return result;
		}
	}
}
