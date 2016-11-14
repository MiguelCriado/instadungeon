[System.Serializable]
public class Layout 
{
    public Zone InitialZone;
    public Zone FinalZone;

    public Graph<Zone> Zones
    {
        get { return zones; }
    }

    private Graph<Zone> zones;

    public Layout()
    {
		zones = new Graph<Zone>();
    }

    public NodeList<Zone> GetAdjacentZones(Zone zone)
    {
		NodeList<Zone> result = null;
		GraphNode<Zone> node = zones.Nodes.FindByValue(zone) as GraphNode<Zone>;

		if (node != null)
		{
			result = node.Neighbors;
		}

        return result;
    }

    public Zone FindZoneByPosition(int2 tilePosition)
    {
		Zone zone;

		for (int i = 0; i < zones.Count; i++)
		{
			zone = zones.Nodes[i].Value;

			if (zone.bounds.Contains(tilePosition))
			{
				return zone;
			}
		}

		return null;
    }

    public void Add(Zone zone)
    {
		if (!zones.Contains(zone))
		{
			zones.AddNode(zone);
		}
    }

    public void ConnectZones(Zone a, Zone b)
    {
		Add(a);
		Add(b);

		zones.AddUndirectedEdge(a, b);
    }
}
