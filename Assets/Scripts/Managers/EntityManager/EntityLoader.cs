using InstaDungeon.Components;
using System.Collections.Generic;
using UnityEngine;

namespace InstaDungeon
{
	public class EntityLoader
	{
		private static readonly string BasePath = "Prefabs/Entities/";

		private Dictionary<string, Entity> prefabsMap;
		private Dictionary<string, List<Entity>> entityPool;

		public EntityLoader()
		{
			prefabsMap = new Dictionary<string, Entity>();
		}

		public Entity Spawn(string entityName, Transform parent)
		{
			Entity result = null;
			Entity prefab = LoadEntityPrefab(entityName);

			if (prefab != null)
			{
				result = GameObject.Instantiate(prefab, parent);
			}

			return result;
		}

		public bool Dispose(Entity entity)
		{
			// TODO : store in pool

			GameObject.Destroy(entity.gameObject);
			return true;
		}

		protected Entity LoadEntityPrefab(string entityName)
		{
			Entity result;

			if (!prefabsMap.TryGetValue(entityName, out result))
			{
				result = Resources.Load<Entity>(string.Format("{0}{1}", BasePath, entityName));

				if (result == null)
				{
					Locator.Log.Error(string.Format("Entity \"{0}\" not found.", entityName));
				}
			}

			return result;
		}
	}
}
