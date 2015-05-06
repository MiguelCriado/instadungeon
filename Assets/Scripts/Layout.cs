using UnityEngine;
using System.Collections;

[System.Serializable]
public class Layout 
{
    public LayoutZone InitialZone;
    public LayoutZone FinalZone;
    public NodeList<LayoutZone> Zones
    {
        get { return zones.Nodes; }
    }

    private Graph<LayoutZone> zones;

    public Layout()
    {
        zones = new Graph<LayoutZone>();
    }

    public NodeList<LayoutZone> GetAdjacentZones(LayoutZone zone)
    {
        return ((GraphNode<LayoutZone>)zones.Nodes.FindByValue(zone)).Neighbors;
    }
}
