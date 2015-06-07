using System;
using NUnit.Framework;

public class LayoutTest
{
    [Test]
    public void AddOneZone()
    {
        Layout layout = new Layout();
        LayoutZone zone = new LayoutZone();
        layout.Add(zone);
        Assert.True(layout.Zones.Contains(zone) && layout.Zones.Count == 1);
    }

    [Test]
    public void AddTwoZones()
    {
        Layout layout = new Layout();
        LayoutZone zone1 = new LayoutZone();
        LayoutZone zone2 = new LayoutZone();
        layout.Add(zone1);
        layout.Add(zone2);
        Assert.True(layout.Zones.Count == 2
                    && layout.Zones.Contains(zone1)
                    && layout.Zones.Contains(zone2));

    }

    [Test]
    public void ConnectTwoZones()
    {
        Layout layout = new Layout();
        LayoutZone zone1 = new LayoutZone();
        LayoutZone zone2 = new LayoutZone();
        layout.ConnectZones(zone1, zone2);
        Assert.True(layout.GetAdjacentZones(zone1).Count == 1
                    && layout.GetAdjacentZones(zone1).Contains(zone2)
                    && layout.GetAdjacentZones(zone2).Count == 1
                    && layout.GetAdjacentZones(zone2).Contains(zone1));
    }

    [Test]
    public void RepeatTwoZonesConnection()
    {
        Layout layout = new Layout();
        LayoutZone zone1 = new LayoutZone();
        LayoutZone zone2 = new LayoutZone();
        layout.ConnectZones(zone1, zone2);
        layout.ConnectZones(zone2, zone1);
        Assert.True(layout.GetAdjacentZones(zone1).Count == 1
                    && layout.GetAdjacentZones(zone1).Contains(zone2)
                    && layout.GetAdjacentZones(zone2).Count == 1
                    && layout.GetAdjacentZones(zone2).Contains(zone1));
    }

    [Test]
    public void AddAndConnectTwoZones()
    {
        Layout layout = new Layout();
        LayoutZone zone1 = new LayoutZone();
        LayoutZone zone2 = new LayoutZone();
        layout.Add(zone1);
        layout.Add(zone2);
        layout.ConnectZones(zone1, zone2);
        layout.ConnectZones(zone2, zone1);
        Assert.True(layout.GetAdjacentZones(zone1).Count == 1
                    && layout.GetAdjacentZones(zone1).Contains(zone2)
                    && layout.GetAdjacentZones(zone2).Count == 1
                    && layout.GetAdjacentZones(zone2).Contains(zone1));
    }

    [Test]
    public void FindZoneByPosition()
    {
        Layout layout = new Layout();
        LayoutZone zone1 = new LayoutZone(0, 0, 10, 10);
        LayoutZone zone2 = new LayoutZone(10, 10, 10, 10);
        layout.ConnectZones(zone1, zone2);
        Assert.True(layout.FindZoneByPosition(new Vector2Int(5, 5)) == zone1);
    }

    [Test]
    public void FindEdgyZoneByPosition()
    {
        Layout layout = new Layout();
        LayoutZone zone1 = new LayoutZone(0, 0, 10, 10);
        LayoutZone zone2 = new LayoutZone(0, 10, 10, 10);
        layout.ConnectZones(zone1, zone2);
        Assert.True(layout.FindZoneByPosition(new Vector2Int(9, 19)) == zone2);
    }

    [Test]
    public void FindZoneByPosition_OuterPoint()
    {
        Layout layout = new Layout();
        LayoutZone zone1 = new LayoutZone(0, 0, 10, 10);
        LayoutZone zone2 = new LayoutZone(0, 10, 10, 10);
        layout.ConnectZones(zone1, zone2);
        Assert.IsNull(layout.FindZoneByPosition(new Vector2Int(-10, 19)));
    }
}