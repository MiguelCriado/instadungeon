using UnityEngine;

namespace InstaDungeon
{
	public class CameraManager : MonoBehaviour
	{
		public Transform Target { get { return target; } set { target = value; }  }

		[Header("References")]
		[SerializeField]
		private Transform target;        // Reference to the target's transform.
		[Header("Configuration")]
		[SerializeField]
		private float xMargin = 1f;      // Distance in the x axis the target can move before the camera follows.
		[SerializeField]
		private float yMargin = 1f;      // Distance in the y axis the target can move before the camera follows.
		[SerializeField]
		private float xSmooth = 8f;      // How smoothly the camera catches up with it's target movement in the x axis.
		[SerializeField]
		private float ySmooth = 8f;      // How smoothly the camera catches up with it's target movement in the y axis.
		[SerializeField]
		private Vector2 maxXAndY;        // The maximum x and y coordinates the camera can have.
		[SerializeField]
		private Vector2 minXAndY;        // The minimum x and y coordinates the camera can have.

		void Update()
		{
			TrackTarget();
		}

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
	}
}
