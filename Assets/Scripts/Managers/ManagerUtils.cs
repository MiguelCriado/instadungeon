using UnityEngine;

namespace InstaDungeon
{
	public static class ManagerUtils
	{
		public static Transform GetOrCreateManagersParent()
		{
			GameObject result = GameObject.FindGameObjectWithTag("Managers");

			if (result != null)
			{
				result = GameObject.Find("Managers");
			}

			if (result == null)
			{
				result = new GameObject("Managers");
				result.tag = "Managers";
			}

			return result.transform;
		}
	}
}
