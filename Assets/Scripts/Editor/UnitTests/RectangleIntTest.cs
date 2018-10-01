using System;
using NUnit.Framework;

public class RectangleIntTest {

	[Test]
    public void Contains()
    {
        RectangleInt rect = new RectangleInt(0, 0, 5, 5);
        int2 tile = new int2(3, 3);

        Assert.IsTrue(rect.Contains(tile));
    }

    [Test]
    public void NotContains()
    {
        RectangleInt rect = new RectangleInt(0, 0, 5, 5);
		int2 tile = new int2(10, 10);

        Assert.False(rect.Contains(tile));
    }

    [Test]
    public void Overlaps_NE()
    {
        RectangleInt a = new RectangleInt(0, 0, 5, 5);
        RectangleInt b = new RectangleInt(3, 3, 3, 3);

        Assert.IsTrue(a.Overlaps(b));
    }

    [Test]
    public void Overlaps_SE()
    {
        RectangleInt a = new RectangleInt(0, 0, 5, 5);
        RectangleInt b = new RectangleInt(3, -1, 3, 3);

        Assert.IsTrue(a.Overlaps(b));
    }

    [Test]
    public void Overlaps_N()
    {
        RectangleInt a = new RectangleInt(0, 0, 5, 5);
        RectangleInt b = new RectangleInt(1, 3, 3, 3);

        Assert.IsTrue(a.Overlaps(b));
    }

    [Test]
    public void ContactArea_N()
    {
        RectangleInt a = new RectangleInt(0, 0, 5, 5);
        RectangleInt b = new RectangleInt(1, 5, 3, 3);

        Assert.IsTrue(a.ContactArea(b).Contains(new int2(1, 4))
                        && a.ContactArea(b).Contains(new int2(2, 4))
                        && a.ContactArea(b).Contains(new int2(3, 4))
                        && a.ContactArea(b).Count == 3);
    }

    [Test]
    public void ContactArea_E()
    {
        RectangleInt a = new RectangleInt(0, 0, 5, 5);
        RectangleInt b = new RectangleInt(5, 1, 3, 3);

        Assert.IsTrue(a.ContactArea(b).Contains(new int2(4, 1))
                        && a.ContactArea(b).Contains(new int2(4, 2))
                        && a.ContactArea(b).Contains(new int2(4, 3))
                        && a.ContactArea(b).Count == 3);
    }

    [Test]
    public void ContactArea_S()
    {
        RectangleInt a = new RectangleInt(0, 0, 5, 5);
        RectangleInt b = new RectangleInt(1, -3, 3, 3);

        Assert.IsTrue(a.ContactArea(b).Contains(new int2(1, 0))
                        && a.ContactArea(b).Contains(new int2(2, 0))
                        && a.ContactArea(b).Contains(new int2(3, 0))
                        && a.ContactArea(b).Count == 3);
    }

    [Test]
    public void ContactArea_W()
    {
        RectangleInt a = new RectangleInt(0, 0, 5, 5);
        RectangleInt b = new RectangleInt(-3, 1, 3, 3);

        Assert.IsTrue(a.ContactArea(b).Contains(new int2(0, 1))
                        && a.ContactArea(b).Contains(new int2(0, 2))
                        && a.ContactArea(b).Contains(new int2(0, 3))
                        && a.ContactArea(b).Count == 3);
    }

    [Test]
    public void ContactArea_NE()
    {
        RectangleInt a = new RectangleInt(0, 0, 5, 5);
        RectangleInt b = new RectangleInt(-1, 5, 3, 3);

        Assert.IsTrue(a.ContactArea(b).Contains(new int2(0, 4))
                        && a.ContactArea(b).Contains(new int2(1, 4)));
    }

    [Test]
    public void ContactArea_N_DiscardEdges()
    {
        RectangleInt a = new RectangleInt(0, 0, 5, 5);
        RectangleInt b = new RectangleInt(1, 5, 3, 3);

        Assert.IsTrue(a.ContactArea(b, true).Contains(new int2(2, 4))
                        && a.ContactArea(b, true).Count == 1);
    }

    [Test]
    public void ContactArea_E_DiscardEdges()
    {
        RectangleInt a = new RectangleInt(0, 0, 5, 5);
        RectangleInt b = new RectangleInt(5, 1, 3, 3);

        Assert.IsTrue(a.ContactArea(b, true).Contains(new int2(4, 2))
                        && a.ContactArea(b, true).Count == 1);
    }

    [Test]
    public void ContactArea_S_DiscardEdges()
    {
        RectangleInt a = new RectangleInt(0, 0, 5, 5);
        RectangleInt b = new RectangleInt(1, -3, 3, 3);

        Assert.IsTrue(a.ContactArea(b, true).Contains(new int2(2, 0))
                        && a.ContactArea(b, true).Count == 1);
    }

    [Test]
    public void ContactArea_W_DiscardEdges()
    {
        RectangleInt a = new RectangleInt(0, 0, 5, 5);
        RectangleInt b = new RectangleInt(-3, 1, 3, 3);

        Assert.IsTrue(a.ContactArea(b, true).Contains(new int2(0, 2))
                        && a.ContactArea(b, true).Count == 1);
    }
}
