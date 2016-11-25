using Random = UnityEngine.Random;

public static class ZoneConnector
{
	public static TileMap<TileType> BuildMap(ILayoutGenerator layoutGenerator, IZoneGenerator zoneGenerator, int levelSeed)
	{
		Random.InitState(levelSeed);
		TileMap<TileType> result = new TileMap<TileType>();

		Layout layout = layoutGenerator.NewLayout();
		result.Layout = layout;

		while (!layoutGenerator.IsDone())
		{
			result.Layout = layoutGenerator.Iterate(layout);

			result = zoneGenerator.PreConnectZones(result);
			result = zoneGenerator.Generate(result);
			result = zoneGenerator.PostConnectZones(result);
		}

		result.SpawnPoint = zoneGenerator.PlaceStairs(layout.InitialZone, result);
		result.ExitPoint = zoneGenerator.PlaceStairs(layout.FinalZone, result);

		return result;
	}
}
