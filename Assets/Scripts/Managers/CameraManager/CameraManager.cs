using InstaDungeon.Components;
using RSG;
using UnityEngine;

namespace InstaDungeon
{
	public class CameraManager : Manager
	{
		private CameraController camera;

		#region [Public API]

		public void SetTarget(Transform target)
		{
			GetCamera().Target = target;
		}

		public void MoveTo(Vector2 position)
		{
			GetCamera().MoveTo(position);
		}

		public IPromise FadeIn(float duration)
		{
			return GetCamera().FadeIn(duration);
		}

		public IPromise FadeOut(float duration)
		{
			return GetCamera().FadeOut(duration);
		}

		#endregion

		#region [Public API]

		private CameraController GetCamera()
		{
			if (camera == null)
			{
				camera = Object.FindObjectOfType<CameraController>();

				if (camera == null)
				{
					Camera mainCamera = Camera.main;

					if (mainCamera == null)
					{
						mainCamera = Object.FindObjectOfType<Camera>();
					}

					camera = mainCamera.gameObject.AddComponent<CameraController>();
				}
			}

			return camera;
		}

		#endregion
	}
}
