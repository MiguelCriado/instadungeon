using UnityEngine;
using System.Collections;
using System;

public static class IDTools {



    public static Vector2 IsoToCartesian(float x, float y)
    {
        Vector2 result = new Vector2();
        result.x = (2 * y + x) / 2;
        result.y = (2 * y - x) / 2;
        return result;
    }

    public static Vector2 CartesianToIso(float x, float y)
    {
        Vector2 result = new Vector2();
        result.x = /*(tileWidth / 2) **/ (x - y);
        result.y = /*(tileHeight / 2) * */(x + y) / 2;
        return result;
    }

    public static Vector2 PointToTile(float x, float y, float tileHeight = 0.5f)
    {
        Vector2 result = new Vector2(Mathf.Floor(x + 0.5f), Mathf.Floor(y + 0.5f));
        return result;
    }

    private static void Swap<T>(ref T lhs, ref T rhs) {
        T temp;
        temp = lhs;
        lhs = rhs;
        rhs = temp; 
    }

    public delegate bool PlotFunction(int x, int y);

    public static void Line(int x0, int y0, int x1, int y1, PlotFunction plot)
    {
        bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);

        if (steep)
		{
			Swap(ref x0, ref y0);
			Swap(ref x1, ref y1);
		}

        if (x0 > x1)
		{
			Swap(ref x0, ref x1);
			Swap(ref y0, ref y1);
		}

		int dX = (x1 - x0);
		int dY = Math.Abs(y1 - y0);
		int err = (dX / 2);
		int ystep = (y0 < y1 ? 1 : -1);
		int y = y0;

        for (int x = x0; x <= x1; ++x)
        {
			if (!(steep ? plot(y, x) : plot(x, y)))
			{
				return;
			}

            err = err - dY;

            if (err < 0)
			{
				y += ystep;
				err += dX;
			}
        }
    }

    public static int ManhattanDistance(Vector2Int origin, Vector2Int destiny)
    {
        int result = Math.Abs((destiny.x - origin.x)) + Math.Abs((destiny.y - origin.y));
        return result;
    }
}
