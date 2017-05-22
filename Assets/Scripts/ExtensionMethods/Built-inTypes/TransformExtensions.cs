using UnityEngine;

namespace InstaDungeon
{
	public static class TransformExtensions
	{
		public static Transform GetOrCreateContainer(this Transform transform, string name)
		{
			Transform result = transform.Find(name);

			if (result == null)
			{
				GameObject go = new GameObject(name);
				go.transform.SetParent(transform);
				go.transform.localPosition = Vector3.zero;
				result = go.transform;
			}

			return result;
		}
	}
}
