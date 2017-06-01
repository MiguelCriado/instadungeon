using UnityEngine;
using DG.Tweening;
using RSG;

namespace InstaDungeon.Components
{
	public class CameraController : MonoBehaviour
	{
		public Transform Target { get { return target; } set { target = value; } }

		[Header("References")]
		[SerializeField] private Transform target;
		[SerializeField] private Material TransitionMaterial;
		[Header("Configuration")]
		[SerializeField] private float xMargin = 0f;
		[SerializeField] private float yMargin = 0f;
		[SerializeField] private float xSmooth = 2f;
		[SerializeField] private float ySmooth = 2f;
		[SerializeField] private Vector2 maxXAndY = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
		[SerializeField] private Vector2 minXAndY = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

		private float fadeAmount;

		#region [MonoBehaviour Magic Methods]

		private void Awake()
		{
			fadeAmount = 0;
			DOTween.Init();
		}

		private void Update()
		{
			TrackTarget();
		}

		private void OnRenderImage(RenderTexture src, RenderTexture dst)
		{
			if (TransitionMaterial != null)
			{
				RenderTexture rt = RenderTexture.GetTemporary(src.width, src.height);
				rt.filterMode = FilterMode.Point;

				TransitionMaterial.SetFloat("_Fade", fadeAmount);

				Graphics.Blit(src, rt, TransitionMaterial);
				Graphics.Blit(rt, dst);
				RenderTexture.ReleaseTemporary(rt);
			}
		}

		#endregion

		#region [Public API]

		public IPromise FadeIn(float duration)
		{
			return Fade(0f, duration);
		}

		public IPromise FadeOut(float duration)
		{
			return Fade(1f, duration);
		}

		public void MoveTo(Vector2 position)
		{
			transform.position = new Vector3(position.x, position.y, transform.position.z);
		}

		#endregion

		#region [Helpers]

		private void TrackTarget()
		{
			if (target != null)
			{
				Vector3 cameraPosition = transform.position;
				Vector3 targetPosition = target.position;

				float targetX = cameraPosition.x;
				float targetY = cameraPosition.y;

				if (CheckXMargin(cameraPosition, targetPosition))
				{
					targetX = Mathf.Lerp(cameraPosition.x, targetPosition.x, xSmooth * Time.deltaTime);
				}

				if (CheckYMargin(cameraPosition, targetPosition))
				{
					targetY = Mathf.Lerp(cameraPosition.y, targetPosition.y, ySmooth * Time.deltaTime);
				}

				targetX = Mathf.Clamp(targetX, minXAndY.x, maxXAndY.x);
				targetY = Mathf.Clamp(targetY, minXAndY.y, maxXAndY.y);

				transform.position = new Vector3(targetX, targetY, transform.position.z);
			}
		}

		private bool CheckXMargin(Vector3 cameraPosition, Vector3 targetPosition)
		{
			return Mathf.Abs(cameraPosition.x - targetPosition.x) > xMargin;
		}

		private bool CheckYMargin(Vector3 cameraPosition, Vector3 targetPosition)
		{
			return Mathf.Abs(transform.position.y - target.position.y) > yMargin;
		}

		private IPromise Fade(float endValue, float duration)
		{
			Promise result = new Promise();

			DOTween.To
			(
				() => fadeAmount,
				(x) => fadeAmount = x,
				endValue,
				duration
			)
			.OnComplete
			(
				() =>
				{
					result.Resolve();
				}
			);

			return result;
		}

		#endregion
	}
}
