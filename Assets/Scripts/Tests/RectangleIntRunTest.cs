using UnityEngine;
using System.Collections;

public class RectangleIntRunTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        RectangleInt a = new RectangleInt(0, 0, 5, 5);
        RectangleInt b = new RectangleInt(1, -3, 3, 3);

        a.ContactArea(b);

        
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
