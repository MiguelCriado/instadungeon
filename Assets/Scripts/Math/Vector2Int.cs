using System.Collections;
using System;
using UnityEngine;
using Object = System.Object;

[System.Serializable]
public class Vector2Int
{
    [SerializeField]
    public readonly int x;
    [SerializeField]
    public readonly int y;

    public Vector2Int(int x, int y)
    {
        this.x = x;
        this.y = y;
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
        return (x == p.x) && (y == p.y);
    }

    public override int GetHashCode()
    {
        int hash = 13;
        hash = (hash * 7) + x;
        hash = (hash * 7) + y;
        return hash;
    }

    public override string ToString()
    {
        return "(" + x + ", " + y + ")";
    }
}
