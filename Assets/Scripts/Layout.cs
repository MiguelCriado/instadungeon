using UnityEngine;
using System.Collections;

[System.Serializable]
public class Layout 
{
    public LayoutZone InitialZone;
    public LayoutZone FinalZone;

    private Graph<LayoutZone> zones;

    public Layout()
    {
        zones = new Graph<LayoutZone>();
    }
}
