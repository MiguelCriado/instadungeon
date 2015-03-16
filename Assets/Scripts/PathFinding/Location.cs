using System.Collections;
using System;

[System.Serializable]
public class Location
{
    // Implementation notes: I am using the default Equals but it can
    // be slow. You'll probably want to override both Equals and
    // GetHashCode in a real project.
    public readonly int x, y;
    public Location(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override bool Equals(Object obj) {
        // If parameter is null return false.
        if (obj == null)
        {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        Location p = obj as Location;
        if ((System.Object)p == null)
        {
            return false;
        }

        // Return true if the fields match:
        return (x == p.x) && (y == p.y);
    }

    public override string ToString()
    {
        return "(" + x + ", " + y + ")";
    }

    public override int GetHashCode()
    {
        int hash = 13;
        hash = (hash * 7) + x;
        hash = (hash * 7) + y;
        return hash;
    }
}
