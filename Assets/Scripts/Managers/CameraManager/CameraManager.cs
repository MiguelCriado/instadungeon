using InstaDungeon.Components;
using RSG;
using UnityEngine;

namespace InstaDungeon
{
	public class CameraManager : Manager
	{
		private CameraController camera;

		public CameraManager() : base()
		{
			camera = GameObject.FindObjectOfType<CameraController>();

			if (camera == null)
			{
				Camera mainCamera = Camera.main;

				if (mainCamera == null)
				{
					mainCamera = GameObject.FindObjectOfType<Camera>();
				}

				camera = mainCamera.gameObject.AddComponent<CameraController>();
			}
		}

		#region [Public API]

		public void SetTarget(Transform target)
		{
			camera.Target = target;
		}

		public void MoveTo(Vector2 position)
		{
			camera.MoveTo(position);
		}

		public IPromise FadeIn(float duration)
		{
			return camera.FadeIn(duration);
		}

		public IPromise FadeOut(float duration)
		{
			return camera.FadeOut(duration);
		}

		#endregion
	}
}
