using UnityEngine;
using System.Collections;

public class CameraHeightCorrector : MonoBehaviour {

	public float cameraHeight = 10f;
	public float screenHeight;
	public float mX;
	public float pixelsPerUnit = 32;


	// Use this for initialization
	void Start () {
		screenHeight = Screen.height;
		//float x = Screen.height / (cameraHeight*pixelsPerUnit);
		GetComponent<Camera>().orthographicSize = Screen.height/(pixelsPerUnit*mX);
	}
	
	// Update is called once per frame
	void Update () {
        screenHeight = Screen.height;
		GetComponent<Camera>().orthographicSize = Screen.height/(pixelsPerUnit*mX);
	}
}
