using InstaDungeon.Components;
using InstaDungeon.Events;
using System.Collections.Generic;

namespace InstaDungeon
{
	public enum VisibilityType
	{
		Obscured,
		PreviouslySeen,
		Visible
	}

	public class VisibilityManager : Manager
	{
		protected GameManager gameManager;
		protected MapManager mapManager;
		protected EntityManager entityManager;
		protected List<Entity> lightCasters;
		protected Dictionary<Entity, int2> movingLightCasters;

		public VisibilityManager() : base(true, true)
		{
			gameManager = Locator.Get<GameManager>();
			mapManager = Locator.Get<MapManager>();
			entityManager = Locator.Get<EntityManager>();
			lightCasters = new List<Entity>();
			movingLightCasters = new Dictionary<Entity, int2>();
			AddPresentLightCasters();
			entityManager.Events.AddListener(OnEntitySpawned, EntitySpawnEvent.EVENT_TYPE);
		}

		public void Refresh()
		{
			for (int i = 0; i < lightCasters.Count; i++)
			{
				RefreshVisibility(lightCasters[i].CellTransform.Position);
			}
		}

		protected override void OnUpdate()
		{
			var enumerator = movingLightCasters.GetEnumerator();

			List<Entity> entitiesToUpdate = new List<Entity>();

			while (enumerator.MoveNext())
			{
				Entity entity = enumerator.Current.Key;
				int2 lastPosition = enumerator.Current.Value;
				int2 currentPosition = gameManager.Renderer.WorldToTileMapPosition(entity.transform.position);

				if (lastPosition != currentPosition)
				{
					RefreshVisibility(currentPosition);
					gameManager.Renderer.RefreshVisibility();
					entitiesToUpdate.Add(entity);
				}
			}

			for (int i = 0; i < entitiesToUpdate.Count; i++)
			{
				int2 currentPosition = gameManager.Renderer.WorldToTileMapPosition(entitiesToUpdate[i].transform.position);
				movingLightCasters[entitiesToUpdate[i]] = currentPosition;
			}
		}

		protected Shadow ProjectTile(int row, int col)
		{
			float topLeft = (float) col / (row + 2);
			float bottomRight = (float) (col + 1) / (row + 1);
			return new Shadow(topLeft, bottomRight);
		}

		protected int2 TransformOctant(int row, int col, int octant)
		{
			switch (octant)
			{
				default:
				case 0: return new int2( col, -row);
				case 1: return new int2( row, -col);
				case 2: return new int2( row,  col);
				case 3: return new int2( col,  row);
				case 4: return new int2(-col,  row);
				case 5: return new int2(-row,  col);
				case 6: return new int2(-row, -col);
				case 7: return new int2(-col, -row);
			}
		}

		protected void RefreshVisibility(int2 hero)
		{
			if (mapManager.Map != null)
			{
				for (int octant = 0; octant < 8; octant++)
				{
					RefreshOctant(hero, octant);
				}
			}
		}

		protected void RefreshOctant(int2 hero, int octant)
		{
			ShadowLine line = new ShadowLine();
			bool fullShadow = false;
			TileMap<Cell> map = mapManager.Map;

			for (int row = 1; ; row++)
			{
				int2 pos = hero + TransformOctant(row, 0, octant);

				if (!map.Bounds.Contains(pos))
				{
					break;
				}

				for (int col = 0; col <= row; col++)
				{
					pos = hero + TransformOctant(row, col, octant);

					if (!map.Bounds.Contains(pos))
					{
						break;
					}

					Cell cell = map[pos];

					if (cell != null)
					{
						if (fullShadow)
						{
							cell.RefreshVisibility(false);
						}
						else
						{
							Shadow projection = ProjectTile(row, col);

							bool visible = !line.IsInShadow(projection);
							cell.RefreshVisibility(visible);

							if (visible && map[pos].BreaksLineOfSight())
							{
								line.Add(projection);
								fullShadow = line.IsFullShadow;
							}
						}
					}
				}
			}
		}

		#region [Event Reactions]

		private void OnEntitySpawned(IEventData eventData)
		{
			EntitySpawnEvent spawnEvent = eventData as EntitySpawnEvent;

			Entity entity = entityManager.Get(spawnEvent.EntityId);

			if (entity != null)
			{
				RegisterLightCaster(entity);
			}
		}

		private void OnLightSourceAddToMap(IEventData eventData)
		{
			EntityAddToMapEvent addToMapEvent = eventData as EntityAddToMapEvent;
			Entity entity = lightCasters.Find(x => x.Guid == addToMapEvent.Entity.Guid);

			if (entity != null)
			{
				RefreshLightPoint(entity);
			}
		}

		private void OnLightSourceStartMoving(IEventData eventData)
		{
			EntityStartMovementEvent movementEvent = eventData as EntityStartMovementEvent;
			Entity entity = lightCasters.Find(x => x.Guid == movementEvent.EntityId);

			if (entity != null)
			{
				movingLightCasters.Add(entity, entity.CellTransform.Position);
			}
		}

		private void OnLightSourceFinishMoving(IEventData eventData)
		{
			EntityFinishMovementEvent movementEvent = eventData as EntityFinishMovementEvent;
			Entity entity = lightCasters.Find(x => x.Guid == movementEvent.EntityId); ;

			if (entity != null && movingLightCasters.ContainsKey(entity))
			{
				movingLightCasters.Remove(entity);
			}
		}

		private void OnDoorOpens(IEventData eventData)
		{
			DoorOpenEvent doorEvent = eventData as DoorOpenEvent;
			Entity door = doorEvent.Door;
			Cell cell = mapManager.Map[door.CellTransform.Position];

			if (cell != null && cell.Visibility == VisibilityType.Visible)
			{
				Refresh();
				gameManager.Renderer.RefreshVisibility(); // TODO: switch this to a reactive system using VisibilityRefreshedEvent 
			}
		}

		#endregion

		#region [Helpers]

		private void RegisterLightCaster(Entity entity)
		{
			LightCaster lightCaster = entity.GetComponent<LightCaster>();

			if (lightCaster != null)
			{
				lightCasters.Add(entity);
				RefreshVisibility(entity.CellTransform.Position);
				gameManager.Renderer.RefreshVisibility();

				entity.Events.AddListener(OnLightSourceAddToMap, EntityAddToMapEvent.EVENT_TYPE);
				entity.Events.AddListener(OnLightSourceStartMoving, EntityStartMovementEvent.EVENT_TYPE);
				entity.Events.AddListener(OnLightSourceFinishMoving, EntityFinishMovementEvent.EVENT_TYPE);
			}

			if (entity.Info.NameId == "Door") // TODO: work a scriptableObject manager to access them via code
			{
				entity.Events.AddListener(OnDoorOpens, DoorOpenEvent.EVENT_TYPE);
			}
		}

		private void AddPresentLightCasters()
		{
			List<Entity> presentEntities = entityManager.Entities;

			for (int i = 0; i < presentEntities.Count; i++)
			{
				RegisterLightCaster(presentEntities[i]);
			}

			List<Entity> mapEntities = new List<Entity>();
			mapEntities.AddRange(mapManager.GetActors());
			mapEntities.AddRange(mapManager.GetProps());
			mapEntities.AddRange(mapManager.GetItems());

			for (int i = 0; i < mapEntities.Count; i++)
			{
				LightCaster lightCaster = mapEntities[i].GetComponent<LightCaster>();

				if (lightCaster != null)
				{
					RefreshLightPoint(mapEntities[i]);
				}
			}
		}

		private void RefreshLightPoint(Entity entity)
		{
			int2 pos = entity.CellTransform.Position;
			Cell cell = mapManager.Map[pos];

			if (cell != null)
			{
				cell.RefreshVisibility(true);
				RefreshVisibility(entity.CellTransform.Position);
				gameManager.Renderer.RefreshVisibility();
			}
		}

		#endregion
	}
}
