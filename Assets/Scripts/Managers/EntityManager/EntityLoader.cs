using InstaDungeon.Components;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace InstaDungeon
{
	public class EntityLoader : MonoBehaviour
	{
		private static readonly string BASE_PATH = "Assets/Prefabs/Entities/";

		[Header("References")]
		[SerializeField]
		private Entity[] availableEntities;
		[Header("Options")]
		[SerializeField]
		private bool loadAllEntities;

		private Dictionary<string, Entity> prefabsMap;
		private Dictionary<string, List<Entity>> entityPool;

		void Awake()
		{
			prefabsMap = new Dictionary<string, Entity>();

			for (int i = 0; i < availableEntities.Length; i++)
			{
				prefabsMap.Add(availableEntities[i].name, availableEntities[i]);
			}
		}

		public Entity Spawn(string entityName, Transform parent)
		{
			Entity result;

			if (prefabsMap.TryGetValue(entityName, out result))
			{
				// TODO : get from pool
				return Instantiate(result, parent) as Entity;
			}
			else
			{
				Locator.Log.Error(string.Format("Entity \"{0}\" not found.", entityName));
				return null;
			}
		}

		public bool Dispose(Entity entity)
		{
			// TODO : store in pool

			Destroy(entity.gameObject);
			return true;
		}

		private void LoadEntities()
		{
			List<Entity> entities = new List<Entity>();

			if (loadAllEntities)
			{
				entities = LoadEntitiesAtPath(BASE_PATH);
			}

			availableEntities = entities.ToArray();
		}

		private List<Entity> LoadEntitiesAtPath(string path, bool includeSubFolders = false)
		{
			List<Entity> result = new List<Entity>();

			Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);

			for (int i = 0; i < assets.Length; i++)
			{
				if (assets[i] is GameObject)
				{
					Entity entity = ((GameObject)assets[i]).GetComponent<Entity>();

					if (entity != null)
					{
						result.Add(entity);
					}
				}
			}

			if (includeSubFolders)
			{
				string[] subFolders = AssetDatabase.GetSubFolders(path);

				for (int i = 0; i < subFolders.Length; i++)
				{
					result.AddRange(LoadEntitiesAtPath(Path.Combine(path, subFolders[i])));
				}
			}

			return result;
		}
	}
}
