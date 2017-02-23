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
		protected MapManager mapManager;
		protected EntityManager entityManager;
		protected List<Entity> lightCasters;
		protected Dictionary<Entity, int2> movingLightCasters;

		public VisibilityManager() : base(true, true)
		{
			mapManager = Locator.Get<MapManager>();
			entityManager = Locator.Get<EntityManager>();
			entityManager.Events.AddListener(OnEntitySpawned, EntitySpawnEvent.EVENT_TYPE);
			lightCasters = new List<Entity>();
			movingLightCasters = new Dictionary<Entity, int2>();
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
				int2 currentPosition = GameManager.Renderer.WorldToTileMapPosition(entity.transform.position);

				if (lastPosition != currentPosition)
				{
					RefreshVisibility(currentPosition);
					GameManager.Renderer.RefreshVisibility();
					entitiesToUpdate.Add(entity);
				}
			}

			for (int i = 0; i < entitiesToUpdate.Count; i++)
			{
				int2 currentPosition = GameManager.Renderer.WorldToTileMapPosition(entitiesToUpdate[i].transform.position);
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

		protected void OnEntitySpawned(IEventData eventData)
		{
			EntitySpawnEvent spawnEvent = eventData as EntitySpawnEvent;

			Entity entity = entityManager.Get(spawnEvent.EntityId);

			if (entity != null)
			{
				LightCaster lightCaster = entity.GetComponent<LightCaster>();

				if (lightCaster != null)
				{
					lightCasters.Add(entity);
					RefreshVisibility(entity.CellTransform.Position);
					GameManager.Renderer.RefreshVisibility();

					entity.Events.AddListener(OnLightSourceRelocate, EntityRelocateEvent.EVENT_TYPE);
					entity.Events.AddListener(OnLightSourceStartMoving, EntityStartMovementEvent.EVENT_TYPE);
					entity.Events.AddListener(OnLightSourceFinishMoving, EntityFinishMovementEvent.EVENT_TYPE);
				}
			}
		}

		protected void OnLightSourceRelocate(IEventData eventData)
		{
			EntityRelocateEvent relocateEvent = eventData as EntityRelocateEvent;
			Entity entity = lightCasters.Find(x => x.Guid == relocateEvent.EntityId);

			if (entity != null)
			{
				int2 pos = entity.CellTransform.Position;
				Cell cell = mapManager.Map[pos];

				if (cell != null)
				{
					cell.RefreshVisibility(true);
					RefreshVisibility(entity.CellTransform.Position);
					GameManager.Renderer.RefreshVisibility();
				}
			}
		}

		protected void OnLightSourceStartMoving(IEventData eventData)
		{
			EntityStartMovementEvent movementEvent = eventData as EntityStartMovementEvent;
			Entity entity = lightCasters.Find(x => x.Guid == movementEvent.EntityId);

			if (entity != null)
			{
				movingLightCasters.Add(entity, entity.CellTransform.Position);
			}
		}

		protected void OnLightSourceFinishMoving(IEventData eventData)
		{
			EntityFinishMovementEvent movementEvent = eventData as EntityFinishMovementEvent;
			Entity entity = lightCasters.Find(x => x.Guid == movementEvent.EntityId); ;

			if (entity != null && movingLightCasters.ContainsKey(entity))
			{
				movingLightCasters.Remove(entity);
			}
		}
	}
}
