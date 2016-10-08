using System;
using UnityEngine;
using Object = System.Object;

[Serializable]
public class Vector2Int : IEquatable<Vector2Int>
{
    public int x { get { return _x; } }
    public int y { get { return _y; } }

    [SerializeField] private int _x;
    [SerializeField] private int _y;

    public Vector2Int(int x, int y)
    {
        this._x = x;
        this._y = y;
    }

	public Vector2Int Set(int x, int y)
	{
		_x = x;
		_y = y;

		return this;
	}

    public static Vector2Int operator +(Vector2Int a, Vector2Int b)
    {
        return new Vector2Int(a.x + b.x, a.y + b.y);
    }

    public static Vector2Int operator -(Vector2Int a, Vector2Int b)
    {
        return new Vector2Int(a.x - b.x, a.y - b.y);
    }

    public static bool operator ==(Vector2Int a, Vector2Int b)
    {
        // If both are null, or both are same instance, return true.
        if (System.Object.ReferenceEquals(a, b))
        {
            return true;
        }

        // If one is null, but not both, return false.
        if (((object)a == null) || ((object)b == null))
        {
            return false;
        }

        // Return true if the fields match:
        return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(Vector2Int a, Vector2Int b)
    {
        return !(a == b);
    }

	public bool Equals(Vector2Int other)
	{
		return (_x == other.x) && (_y == other.y);
	}

	public override bool Equals(Object obj)
    {
        // If parameter is null return false.
        if (obj == null)
        {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        Vector2Int p = obj as Vector2Int;
        if ((System.Object)p == null)
        {
            return false;
        }

        // Return true if the fields match:
        return (_x == p.x) && (_y == p.y);
    }

    public override int GetHashCode()
    {
        int hash = 13;
        hash = (hash * 7) + _x;
        hash = (hash * 7) + _y;
        return hash;
    }

    public override string ToString()
    {
        return "(" + _x + ", " + _y + ")";
    }
}
