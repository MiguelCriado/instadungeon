using InstaDungeon.Events;
using InstaDungeon.Models;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace InstaDungeon.Components
{
	[RequireComponent(typeof(Entity))]
	public class LootDropper : MonoBehaviour
	{
		[System.Serializable]
		private class LootEntry
		{
			public string EntityName { get { return entityName; } }
			public int Weight { get { return weight; } }
			public bool AlwaysDrop { get { return alwaysDrop; } }

			[SerializeField] private string entityName;
			[SerializeField] private int weight;
			[SerializeField] private bool alwaysDrop;

			public LootEntry(string entityName, int weight, bool alwaysDrop)
			{
				this.entityName = entityName;
				this.weight = weight;
				this.alwaysDrop = alwaysDrop;
			}
		}

		[SerializeField] private List<LootEntry> lootTable;
		[SerializeField] private List<LootEntry> alwaysDropTable;

		private Entity entity;
		private EntityManager entityManager;
		private MapManager mapManager;

		private void OnValidate()
		{
			LootEntry lootEntry;

			while ((lootEntry = lootTable.Find(x => x.AlwaysDrop == true)) != null)
			{
				alwaysDropTable.Add(lootEntry);
				lootTable.Remove(lootEntry);
			}

			while ((lootEntry = alwaysDropTable.Find(x => x.AlwaysDrop == false)) != null)
			{
				lootTable.Add(lootEntry);
				alwaysDropTable.Remove(lootEntry);
			}
		}

		private void Awake()
		{
			entity = GetComponent<Entity>();
			entityManager = Locator.Get<EntityManager>();
			mapManager = Locator.Get<MapManager>();
		}

		private void Start()
		{
			entity.Events.AddListener(OnEntityDeath, EntityDieEvent.EVENT_TYPE);
		}

		public void AddDrop(string entityName, int weight, bool alwaysDrop)
		{
			if (alwaysDrop)
			{
				alwaysDropTable.Add(new LootEntry(entityName, weight, alwaysDrop));
			}
			else
			{
				lootTable.Add(new LootEntry(entityName, weight, alwaysDrop));
			}
		}

		public void Drop(int2 dropSpot)
		{
			Entity entityToDrop = null;

			if (alwaysDropTable.Count > 0)
			{
				entityToDrop = entityManager.Spawn(alwaysDropTable[0].EntityName);
			}

			if (entityToDrop == null)
			{
				int totalWeight = 0;

				for (int i = 0; i < lootTable.Count; i++)
				{
					totalWeight += lootTable[i].Weight;
				}

				int cumulativeWeight = 0;
				int randomNumber = Random.Range(0, totalWeight);
				int j = 0;

				while (entityToDrop == null && j < lootTable.Count)
				{
					cumulativeWeight += lootTable[j].Weight;

					if (cumulativeWeight >= randomNumber)
					{
						entityToDrop = entityManager.Spawn(lootTable[j].EntityName);
					}

					j++;
				}
			}

			if (entityToDrop != null)
			{
				if (entityToDrop.GetComponent<Item>() != null)
				{
					mapManager.AddItem(entityToDrop, dropSpot);
				}
				else if (entityToDrop.GetComponent<Actor>() != null)
				{
					mapManager.AddActor(entityToDrop, dropSpot);
				}
				else
				{
					mapManager.AddProp(entityToDrop, dropSpot);
				}

				DOTween.Sequence()
				.Append
				(
					entityToDrop.transform.DOLocalMoveY(entityToDrop.transform.localPosition.y + 1f, 0.3f)
					.SetEase(Ease.OutBack)
				)
				.Append
				(
					entityToDrop.transform.DOLocalMoveY(entityToDrop.transform.localPosition.y, 0.4f)
					.SetEase(Ease.OutBounce)
				);
			}
		}

		#region [Event Reactions]

		private void OnEntityDeath(IEventData eventData)
		{
			EntityDieEvent dieEvent = eventData as EntityDieEvent;
			Drop(dieEvent.Entity.CellTransform.Position);
		}

		#endregion
	}
}
