using System;

public static class TileMapExtensions
{
	public static TileMap<T> Convert<U, T>(this TileMap<U> map, Func<U, T> conversionFunction)
	{
		TileMap<T> result = new TileMap<T>();
		Layout resultLayout = new Layout();
		result.Layout = resultLayout;

		// Add zones & tiles

		Graph<Zone> mapGraph = map.Layout.Zones;
		Zone zone;

		for (int i = 0; i < mapGraph.Count; i++)
		{
			zone = mapGraph.Nodes[i].Value;
			RectangleInt bounds = zone.bounds;
			Zone newZone = new Zone(bounds.x, bounds.y, bounds.width, bounds.height);

			var enumerator = zone.GetEnumerator();
			int2 tile;

			while (enumerator.MoveNext())
			{
				tile = enumerator.Current;
				newZone.tiles.Add(tile);
				result.Add(tile, conversionFunction(map[tile.x, tile.y]));
			}

			resultLayout.Add(newZone);
		}

		// Link layout Zones

		Zone resultZone;

		for (int i = 0; i < mapGraph.Count; i++)
		{
			zone = mapGraph.Nodes[i].Value;
			GraphNode<Zone> mapZoneNode = mapGraph.Nodes.FindByValue(zone) as GraphNode<Zone>;
			resultZone = resultLayout.FindZoneByPosition(zone.bounds.position);

			var linkEnumerator = mapZoneNode.Neighbors.GetEnumerator();

			while (linkEnumerator.MoveNext())
			{
				Zone mapConnectionZone = linkEnumerator.Current.Value;
				Zone resultConnectionZone = resultLayout.FindZoneByPosition(mapConnectionZone.bounds.position);

				GraphNode<Zone> resultZoneNode = resultLayout.Zones.Nodes.FindByValue(resultZone) as GraphNode<Zone>;

				if (CountOccurrences(resultZoneNode.Neighbors, resultConnectionZone) < CountOccurrences(mapZoneNode.Neighbors, mapConnectionZone))
				{
					resultLayout.ConnectZones(resultZone, resultConnectionZone);
				}
			}
		}

		// Connect zones

		for (int i = 0; i < mapGraph.Count; i++)
		{
			zone = mapGraph.Nodes[i].Value;
			resultZone = resultLayout.FindZoneByPosition(zone.bounds.position);

			int2 connectionPoint;
			Zone connectionZone;
			var connectionsEnumerator = zone.connections.GetEnumerator();

			while (connectionsEnumerator.MoveNext())
			{
				int2 currentMapPoint = connectionsEnumerator.Current.Key;
				Zone currentMapZoneConnection = connectionsEnumerator.Current.Value;

				connectionPoint = new int2(currentMapPoint.x, currentMapPoint.y);
				connectionZone = resultLayout.FindZoneByPosition(currentMapZoneConnection.bounds.position);

				resultZone.AddConnectionPoint(connectionPoint, connectionZone);
			}
		}

		// Set Initial & Final zones

		resultLayout.InitialZone = resultLayout.FindZoneByPosition(map.Layout.InitialZone.bounds.position);
		resultLayout.FinalZone = resultLayout.FindZoneByPosition(map.Layout.FinalZone.bounds.position);

		return result;
	}

	private static int CountOccurrences<T>(NodeList<T> nodeList, T element)
	{
		int result = 0;

		for (int i = 0; i < nodeList.Count; i++)
		{
			if (nodeList[i].Value.Equals(element))
			{
				result++;
			}
		}

		return result;
	}
}
