using NUnit.Framework;

public class LayoutZoneTest 
{

    [Test]
    public void A_LayoutZone_Creation()
    {
        Zone layout0 = new Zone();
        Zone layout1 = new Zone();
        Assert.True(layout1.id == layout0.id + 1);
    }

    [Test]
    public void LayoutZone_AddConnectionPoint()
    {
        Zone layout = new Zone();
        layout.bounds = new RectangleInt(0, 0, 3, 3);
        Zone layout1 = new Zone();
        layout1.bounds = new RectangleInt(0, 3, 3, 3);
        layout.AddConnectionPoint(new int2(0, 2), layout1);
        Assert.True(layout.connections.Count == 1);
    }

    [Test]
    public void LayoutZone_ContactPoint()
    {
        Zone layout = new Zone();
        layout.bounds = new RectangleInt(0, 0, 3, 3);
        Zone layout1 = new Zone();
        layout1.bounds = new RectangleInt(0, 3, 3, 3);
		int2 contactPoint;
		layout.ContactPoint(new int2(0, 3), out contactPoint);

		Assert.True(contactPoint == new int2(0, 2));
    }

    [Test]
    public void Map2Zone()
    {
        Zone zone = new Zone();
        zone.bounds = new RectangleInt(5, 5, 10, 10);
        Assert.True(zone.MapToZone(new int2(5, 5)) == new int2(0, 0));
    }

    [Test]
    public void Zone2Map()
    {
        Zone zone = new Zone();
        zone.bounds = new RectangleInt(5, 5, 10, 10);
        Assert.True(zone.ZoneToMap(new int2(0, 0)) == new int2(5, 5));
    }
}