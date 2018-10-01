
using System.Collections;

public static class HilbertCurve {

    //convert (x,y) to d
    public static int xy2d(int n, int x, int y)
    {
        int rx, ry, s, d = 0;
        for (s = n / 2; s > 0; s /= 2)
        {
            rx = (x & s) > 0 ? 1 : 0;
            ry = (y & s) > 0 ? 1 : 0;
            d += s * s * ((3 * rx) ^ ry);
            Rot(s, ref x, ref y, rx, ry);
        }
        return d;
    }

    //convert d to (x,y)
    public static int2 d2xy(int n, int d)
    {
        int rx, ry, s, t = d;
        int x = 0,  y = 0;
        for (s = 1; s < n; s *= 2)
        {
            rx = 1 & (t / 2);
            ry = 1 & (t ^ rx);
            Rot(s, ref x, ref y, rx, ry);
            x += s * rx;
            y += s * ry;
            t /= 4;
        }
        return new int2(x, y);
    }

    //rotate/flip a quadrant appropriately
    private static void Rot(int n, ref int x, ref int y, int rx, int ry)
    {
        if (ry == 0)
        {
            if (rx == 1)
            {
                x = n - 1 - x;
                y = n - 1 - y;
            }

            //Swap x and y
            int t = x;
            x = y;
            y = t;
        }
    }
}
