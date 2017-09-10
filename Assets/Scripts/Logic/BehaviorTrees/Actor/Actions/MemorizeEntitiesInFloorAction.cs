using AI.BehaviorTrees;
using InstaDungeon.Components;
using System.Collections.Generic;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class MemorizeEntitiesInFloorAction : ActionNode
	{
		private static readonly int2[] Directions = new int2[] 
		{
			new int2(0, 1),
			new int2(1, 0),
			new int2(0, -1),
			new int2(-1, 0)
		};

		private EntityTypeInMap entityType;
		private string itemsMemoryId;

		public MemorizeEntitiesInFloorAction(EntityTypeInMap entityTypeToMemorize, string itemsMemoryIdInBlackboard)
		{
			entityType = entityTypeToMemorize;
			itemsMemoryId = itemsMemoryIdInBlackboard;
		}

		protected override void Open(Tick tick)
		{
			Dictionary<int2, HashSet<Entity>> memorizedItems;

			if (!tick.Blackboard.TryGet(itemsMemoryId, out memorizedItems))
			{
				memorizedItems = new Dictionary<int2, HashSet<Entity>>();
				tick.Blackboard.Set(itemsMemoryId, memorizedItems);
			}
		}

		protected override NodeStates Tick(Tick tick)
		{
			NodeStates result = NodeStates.Success;

			Dictionary<int2, HashSet<Entity>> memorizedItems;

			if (tick.Blackboard.TryGet(itemsMemoryId, out memorizedItems))
			{
				MapManager mapManager = Locator.Get<MapManager>();
				HashSet<int2> checkedTiles = new HashSet<int2>();
				Entity target = tick.Target as Entity;
				int2 origin = target.CellTransform.Position;
				ScanFloor(origin, memorizedItems, checkedTiles, mapManager);
			}

			return result;
		}

		private void ScanFloor(int2 tile, Dictionary<int2, HashSet<Entity>> memorizeEntities, HashSet<int2> checkedTiles, MapManager mapManager)
		{
			if (!checkedTiles.Contains(tile))
			{
				checkedTiles.Add(tile);
				Cell cell = mapManager[tile.x, tile.y];

				if (cell != null && cell.Visibility == VisibilityType.Visible && cell.TileInfo.TileType == TileType.Floor)
				{
					if (GetEntityCountInCell(cell) > 0)
					{
						if (!memorizeEntities.ContainsKey(tile))
						{
							memorizeEntities.Add(tile, new HashSet<Entity>());
						}

						List<Entity> entityList = GetEntityList(cell);
						memorizeEntities[tile].UnionWith(entityList);
						RemoveNoPresentItemsInTile(tile, memorizeEntities[tile], entityList);

						if (memorizeEntities[tile].Count <= 0)
						{
							memorizeEntities.Remove(tile);
						}
					}

					for (int i = 0; i < Directions.Length; i++)
					{
						ScanFloor(tile + Directions[i], memorizeEntities, checkedTiles, mapManager);
					}
				}
			}
		}

		private int GetEntityCountInCell(Cell cell)
		{
			int result = 0;

			switch (entityType)
			{
				case EntityTypeInMap.Actor: result = cell.Actor != null ? 1 : 0; break;
				case EntityTypeInMap.Prop: result = cell.Prop != null ? 1 : 0; break;
				case EntityTypeInMap.Item: result = cell.Items.Count; break;
			}

			return result;
		}

		private List<Entity> GetEntityList(Cell cell)
		{
			List<Entity> result = new List<Entity>();

			switch (entityType)
			{
				case EntityTypeInMap.Actor:

					if (cell.Actor != null)
					{
						result.Add(cell.Actor);
					}

					break;
				case EntityTypeInMap.Prop:

					if (cell.Prop != null)
					{
						result.Add(cell.Prop);
					}

					break;
				case EntityTypeInMap.Item: result.AddRange(cell.Items); break;
			}

			return result;
		}

		private void RemoveNoPresentItemsInTile(int2 tile, HashSet<Entity> memorizedItemsInTile, List<Entity> itemsInTile)
		{
			var enumerator = memorizedItemsInTile.GetEnumerator();
			HashSet<Entity> itemsToRemove = new HashSet<Entity>();

			while (enumerator.MoveNext())
			{
				if (!itemsInTile.Contains(enumerator.Current))
				{
					itemsToRemove.Add(enumerator.Current);
				}
			}

			memorizedItemsInTile.ExceptWith(itemsToRemove);
		}
	}
}
