using UnityEngine;

namespace InstaDungeon.Components
{
	public class OrderInLayerUpdater : MonoBehaviour
	{
		private SpriteRenderer[] renderers;
		private Vector3 lastPosition;

		private void Awake()
		{
			renderers = GetComponentsInChildren<SpriteRenderer>(true);
			lastPosition = Vector3.one * float.NaN;
		}

		private void Update()
		{
			Vector3 position = transform.position;

			if (position != lastPosition)
			{
				lastPosition = position;
				int orderInLayer = -Mathf.FloorToInt(position.y * 24);

				for (int i = 0; i < renderers.Length; i++)
				{
					renderers[i].sortingOrder = orderInLayer;
				}
			}
		}
	}
}
