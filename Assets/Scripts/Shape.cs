using UnityEngine;
using System.Collections;

[System.Serializable]
public class Shape 
{
    public enum ConnectionTime
    {
        PreConnection,  // The connection needs to be made BEFORE generation
        PostConnection  // The connection needs to be made AFTER generation
    }
}
