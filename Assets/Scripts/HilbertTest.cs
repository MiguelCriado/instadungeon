using UnityEngine;
using System.Collections;

public class HilbertTest : MonoBehaviour {

    public int n = 8;
    public int d = 0;

	// Use this for initialization
	void Start () {
        int x = 0, y = 0;
        HilbertCurve.d2xy(n, d, ref x, ref y);
        Debug.Log("Hilbert distance of " + d + " = (" + x + ", " + y + ")");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
