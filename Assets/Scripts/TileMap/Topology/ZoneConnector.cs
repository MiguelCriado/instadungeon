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

			NodeList<Zone> zones = layout.Zones.Nodes;

			for (int i = 0; i < zones.Count; i++)
			{
				result = zoneGenerator.Generate(zones[i].Value, result);
			}

			result = zoneGenerator.PostConnectZones(result);
		}

		result = zoneGenerator.PlaceStairs(layout.InitialZone, result);
		result = zoneGenerator.PlaceStairs(layout.FinalZone, result);

		return result;
	}
}
