using System.Collections.Generic;

[System.Serializable]
public class Layout 
{
    public LayoutZone InitialZone;
    public LayoutZone FinalZone;

    public ICollection<LayoutZone> Zones
    {
        get { return zones.Keys; }
    }

    private Dictionary<LayoutZone, HashSet<LayoutZone>> zones;

    public Layout()
    {
        zones = new Dictionary<LayoutZone, HashSet<LayoutZone>>();
    }

    public HashSet<LayoutZone> GetAdjacentZones(LayoutZone zone)
    {
        HashSet<LayoutZone> result = null;
        zones.TryGetValue(zone, out result);

        return result;
    }

    public LayoutZone FindZoneByPosition(int2 tilePosition)
    {
        LayoutZone result = null;

        foreach (LayoutZone zone in zones.Keys)
        {
            if (zone.bounds.Contains(tilePosition))
            {
                result = zone;
                break;
            }
        }

        return result;
    }

    public void Add(LayoutZone zone)
    {
        if (!zones.ContainsKey(zone))
        {
            zones.Add(zone, new HashSet<LayoutZone>());
			zone.ParentLayout = this;
        }
    }

    public void ConnectZones(LayoutZone a, LayoutZone b)
    {
        Add(a);
        Add(b);
        AddAdjacentZone(a, b);
        AddAdjacentZone(b, a);
    }

    private void AddAdjacentZone(LayoutZone a, LayoutZone b)
    {
        HashSet<LayoutZone> adjacentZones;

        if (zones.TryGetValue(a, out adjacentZones))
        {
            adjacentZones.Add(b);
        }
    }
}
