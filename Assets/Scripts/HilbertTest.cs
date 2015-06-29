using UnityEngine;
using System.Collections;

public class HilbertTest : MonoBehaviour {

    public Vector2Int vector;

    public int n = 8;
    public int d = 0;

	// Use this for initialization
	void Start () {
        Vector2Int vector = HilbertCurve.d2xy(n, d);
        Debug.Log("Hilbert distance of " + d + " = (" + vector.x + ", " + vector.y + ")");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
