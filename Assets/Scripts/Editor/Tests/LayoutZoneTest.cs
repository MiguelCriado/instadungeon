using NUnit.Framework;

public class LayoutZoneTest 
{

    [Test]
    public void A_LayoutZone_Creation()
    {
        LayoutZone layout0 = new LayoutZone();
        LayoutZone layout1 = new LayoutZone();
        Assert.True(layout1.id == layout0.id + 1);
    }

    [Test]
    public void LayoutZone_AddConnectionPoint()
    {
        LayoutZone layout = new LayoutZone();
        layout.bounds = new RectangleInt(0, 0, 3, 3);
        LayoutZone layout1 = new LayoutZone();
        layout1.bounds = new RectangleInt(0, 3, 3, 3);
        layout.AddConnectionPoint(new int2(0, 2), layout1);
        Assert.True(layout.connections.Count == 1);
    }

    [Test]
    public void LayoutZone_ContactPoint()
    {
        LayoutZone layout = new LayoutZone();
        layout.bounds = new RectangleInt(0, 0, 3, 3);
        LayoutZone layout1 = new LayoutZone();
        layout1.bounds = new RectangleInt(0, 3, 3, 3);
		int2 contactPoint;
		layout.ContactPoint(new int2(0, 3), out contactPoint);

		Assert.True(contactPoint == new int2(0, 2));
    }

    [Test]
    public void Map2Zone()
    {
        LayoutZone zone = new LayoutZone();
        zone.bounds = new RectangleInt(5, 5, 10, 10);
        Assert.True(zone.Map2Zone(new int2(5, 5)) == new int2(0, 0));
    }

    [Test]
    public void Zone2Map()
    {
        LayoutZone zone = new LayoutZone();
        zone.bounds = new RectangleInt(5, 5, 10, 10);
        Assert.True(zone.Zone2Map(new int2(0, 0)) == new int2(5, 5));
    }
}