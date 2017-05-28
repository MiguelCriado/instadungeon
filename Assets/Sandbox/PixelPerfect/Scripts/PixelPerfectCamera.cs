using UnityEngine;
using UnityEngine.Profiling;

public class PixelPerfectCamera : MonoBehaviour
{
	[SerializeField] private int targetHeight = 120;
	[Range(1, 10), SerializeField] private int zoom = 1;
	[SerializeField] private float pixelsPerUnit = 24;

	private Camera targetCamera;
	private int lastScreenHeight;
	[SerializeField] private RenderTexture renderTexture;
	[SerializeField] private RenderTexture nativeRenderTexture;

	private void OnValidate()
	{
		Initialize();
	}

	private void Start()
	{
		Initialize();
	}

	private void Update()
	{
		if (Screen.height != lastScreenHeight)
		{
			RefreshGraphicSettings();
		}

		lastScreenHeight = Screen.height;
	}

	private void OnPreRender()
	{
		targetCamera.targetTexture = renderTexture;
	}

	private void OnPostRender()
	{
		targetCamera.targetTexture = null;

		Profiler.BeginSample("Blitz Operations");
		Graphics.Blit(renderTexture, nativeRenderTexture);
		Graphics.Blit(nativeRenderTexture, null as RenderTexture);
		Profiler.EndSample();
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

		RefreshGraphicSettings();

		lastScreenHeight = Screen.height;
	}

	private void RefreshGraphicSettings()
	{
		UpdateOrthographicSize();
		RebuildRenderTexture();
	}

	private void UpdateOrthographicSize()
	{
		targetCamera.orthographicSize = (targetHeight / (pixelsPerUnit * zoom)) / 2;
	}

	private void RebuildRenderTexture()
	{
		float screenRatio = (float)Screen.width / Screen.height;

		renderTexture = new RenderTexture(Mathf.RoundToInt(targetHeight * screenRatio), targetHeight, 24, RenderTextureFormat.ARGB32);
		renderTexture.filterMode = FilterMode.Point;

		nativeRenderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
		nativeRenderTexture.filterMode = FilterMode.Point;
	}
}
