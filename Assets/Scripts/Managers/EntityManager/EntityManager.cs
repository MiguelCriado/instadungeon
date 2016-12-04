using System.Collections.Generic;
using UnityEngine;
using UniqueId = System.Int32;

namespace InstaDungeon
{
	public class EntityManager : MonoBehaviour
	{
		private Dictionary<UniqueId, Entity> dynamicEntities;

		void Awake()
		{
			dynamicEntities = new Dictionary<UniqueId, Entity>();
		}

		public Entity Spawn(string entityType)
		{
			Entity result = null;

			return result;
		}
	}
}
