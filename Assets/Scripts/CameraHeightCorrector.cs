using UnityEngine;

public class CameraHeightCorrector : MonoBehaviour
{
	[Range(1, 10), SerializeField] private int zoom = 2;
	[SerializeField] private float pixelsPerUnit = 24;

	private Camera targetCamera;
	private int lastScreenHeight;

	void OnValidate()
	{
		Initialize();
	}

	void Start ()
	{
		Initialize();
	}
	
	void Update ()
	{
		if (Screen.height != lastScreenHeight)
		{
			UpdateOrthographicSize();
		}

		lastScreenHeight = Screen.height;
	}

	private void Initialize()
	{
		if (zoom < 1)
		{
			zoom = 1;
		}

		if (targetCamera == null)
		{
			targetCamera = GetComponent<Camera>();
		}

		UpdateOrthographicSize();
		lastScreenHeight = Screen.height;
	}

	private void UpdateOrthographicSize()
	{
		targetCamera.orthographicSize = (Screen.height / (pixelsPerUnit * zoom)) / 2;
	}
}
