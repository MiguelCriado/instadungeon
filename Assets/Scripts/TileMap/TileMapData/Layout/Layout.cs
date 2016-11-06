[System.Serializable]
public class Layout 
{
    public LayoutZone InitialZone;
    public LayoutZone FinalZone;

    public Graph<LayoutZone> Zones
    {
        get { return zones; }
    }

    private Graph<LayoutZone> zones;

    public Layout()
    {
		zones = new Graph<LayoutZone>();
    }

    public NodeList<LayoutZone> GetAdjacentZones(LayoutZone zone)
    {
		NodeList<LayoutZone> result = null;
		GraphNode<LayoutZone> node = zones.Nodes.FindByValue(zone) as GraphNode<LayoutZone>;

		if (node != null)
		{
			result = node.Neighbors;
		}

        return result;
    }

    public LayoutZone FindZoneByPosition(int2 tilePosition)
    {
		LayoutZone zone;

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

    public void Add(LayoutZone zone)
    {
		if (!zones.Contains(zone))
		{
			zones.AddNode(zone);
		}
    }

    public void ConnectZones(LayoutZone a, LayoutZone b)
    {
		Add(a);
		Add(b);

		zones.AddUndirectedEdge(a, b);
    }
}
