using UnityEngine;

namespace InstaDungeon.UI
{
	[RequireComponent(typeof(RectTransform))]
	public class FollowWorldTransform : MonoBehaviour
	{
		public Transform Target { get { return target; } set { target = value; } }
		public Vector2 Offset { get { return offset; } set { offset = value; } }

		[SerializeField] private Camera worldCamera;
		[SerializeField] private Transform target;
		[SerializeField] private Vector2 offset;

		private RectTransform rectTransform;
		private RectTransform parentRectTransform;

		private void Reset()
		{
			offset = new Vector2(0, 24f);
		}

		private void Awake()
		{
			rectTransform = GetComponent<RectTransform>();
			parentRectTransform = transform.parent.GetComponent<RectTransform>();

			if (worldCamera == null)
			{
				worldCamera = Camera.main;
			}
		}

		private void Update()
		{
			RefreshPosition();
		}

		public void RefreshPosition()
		{
			if (target != null)
			{
				Vector2 targetScreenPosition = RectTransformUtility.WorldToScreenPoint(worldCamera, target.position);
				Vector2 newPosition;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, targetScreenPosition, null, out newPosition);
				rectTransform.localPosition = newPosition + offset;
			}
		}
	}
}
