using System.Collections.Generic;
using UnityEngine;

namespace InstaDungeon
{
	public class PrefabLoader<T> where T : Object
	{
		private static readonly string BasePath = "Prefabs/";

		public string RelativePath { get; private set; }

		private Dictionary<string, T> prefabsMap;
		private Dictionary<string, List<T>> prefabPool;

		public PrefabLoader(string relativePath)
		{
			RelativePath = relativePath;
			prefabsMap = new Dictionary<string, T>();
		}

		public T Spawn(string entityName, Transform parent)
		{
			T result = null;
			T prefab = LoadPrefab(entityName);

			if (prefab != null)
			{
				result = Object.Instantiate(prefab, parent);
			}

			return result;
		}

		public bool Dispose(T instance)
		{
			// TODO : store in pool

			if (typeof(Component).IsAssignableFrom(instance.GetType()))
			{
				Object.Destroy((instance as Component).gameObject);
			}
			else
			{
				Object.Destroy(instance);
			}
			
			return true;
		}

		protected T LoadPrefab(string prefabName)
		{
			T result;

			if (!prefabsMap.TryGetValue(prefabName, out result))
			{
				result = Resources.Load<T>(string.Format("{0}{1}/{2}", BasePath, RelativePath, prefabName));

				if (result == null)
				{
					Locator.Log.Error(string.Format("Prefab \"{0}\" not found.", prefabName));
				}
			}

			return result;
		}
	}
}
