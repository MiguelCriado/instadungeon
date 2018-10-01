using NUnit.Framework;

public class TileMapTest
{
	[Test]
	public void ConvertLayoutCount()
	{
		TileMap<int> original = new TileMap<int>();
		Zone zoneA = new Zone(0, 0, 1, 1);
		Zone zoneB = new Zone(1, 0, 1, 1);
		Zone zoneC = new Zone(2, 0, 1, 1);

		original.Layout.Add(zoneA);
		original.Layout.Add(zoneB);
		original.Layout.Add(zoneC);

		original.Layout.ConnectZones(zoneA, zoneB);
		original.Layout.ConnectZones(zoneB, zoneC);
		original.Layout.ConnectZones(zoneC, zoneA);

		original.Layout.InitialZone = zoneA;
		original.Layout.FinalZone = zoneC;

		TileMap<float> conversion = original.Convert((int value) => { return (float)value; });

		Assert.IsTrue(original.Layout.Zones.Count == conversion.Layout.Zones.Count && conversion.Layout.Zones.Count == 3);
	}

	[Test]
	public void ConvertLayoutConnections()
	{
		TileMap<int> original = new TileMap<int>();
		Zone zoneA = new Zone(0, 0, 1, 1);
		Zone zoneB = new Zone(1, 0, 1, 1);
		Zone zoneC = new Zone(2, 0, 1, 1);

		original.Layout.Add(zoneA);
		original.Layout.Add(zoneB);
		original.Layout.Add(zoneC);

		original.Layout.ConnectZones(zoneA, zoneB);
		original.Layout.ConnectZones(zoneB, zoneA);
		original.Layout.ConnectZones(zoneB, zoneC);
		original.Layout.ConnectZones(zoneC, zoneA);

		original.Layout.InitialZone = zoneA;
		original.Layout.FinalZone = zoneC;

		TileMap<float> conversion = original.Convert((int value) => { return (float)value; });

		GraphNode<Zone> originalNodeA = original.Layout.Zones.Nodes.FindByValue(zoneA) as GraphNode<Zone>;
		GraphNode<Zone> conversionNodeA = conversion.Layout.Zones.Nodes.FindByValue(conversion.Layout.InitialZone) as GraphNode<Zone>;

		Assert.IsTrue(originalNodeA.Neighbors.Count == conversionNodeA.Neighbors.Count);
		Assert.IsTrue(conversionNodeA.Neighbors.Count == 3);

		GraphNode<Zone> originalNodeC = original.Layout.Zones.Nodes.FindByValue(zoneC) as GraphNode<Zone>;
		GraphNode<Zone> conversionNodeC = conversion.Layout.Zones.Nodes.FindByValue(conversion.Layout.FinalZone) as GraphNode<Zone>;

		Assert.IsTrue(originalNodeC.Neighbors.Count == conversionNodeC.Neighbors.Count);
		Assert.IsTrue(conversionNodeC.Neighbors.Count == 2);
	}
}
