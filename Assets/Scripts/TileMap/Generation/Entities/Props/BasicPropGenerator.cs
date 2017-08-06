using InstaDungeon.Components;
using Random = UnityEngine.Random;

namespace InstaDungeon
{
	public class BasicPropGenerator : IPropGenerator
	{
		public void AddStairs(MapManager manager)
		{
			EntityManager entityManager = Locator.Get<EntityManager>();
			TileMap<Cell> map = manager.Map;

			int2 entrancePosition = PropGeneratorUtils.FindPlaceForStairs(manager.Map, map.Layout.InitialZone);
			int2 exitPosition = PropGeneratorUtils.FindPlaceForStairs(manager.Map, map.Layout.FinalZone);

			Entity entranceStairs = entityManager.Spawn("Stairs Entrance");
			Entity exitStairs = entityManager.Spawn("Stairs Exit");

			manager.AddProp(entranceStairs, entrancePosition);
			manager.AddProp(exitStairs, exitPosition);

			map.SpawnPoint = entrancePosition;
		}

		public void AddDoors(MapManager manager)
		{
			//EntityManager entityManager = Locator.Get<EntityManager>();
			//TileMap<Cell> map = manager.Map;
			//Zone finalZone = map.Layout.FinalZone;
			//var finalZoneConnections = finalZone.connections.GetEnumerator();

			//while (finalZoneConnections.MoveNext())
			//{
			//	var currentZone = finalZoneConnections.Current;
			//	var currentZoneConnections = currentZone.Value.connections.GetEnumerator();

			//	while (currentZoneConnections.MoveNext())
			//	{
			//		if (currentZoneConnections.Current.Value == finalZone)
			//		{
			//			Entity door = entityManager.Spawn("Door");
			//			manager.AddProp(door, currentZoneConnections.Current.Key);

			//			FaceDirectionComponent faceDirection = door.GetComponent<FaceDirectionComponent>();
			//			faceDirection.Direction = GetDoorDirection(currentZoneConnections.Current.Key, currentZone.Key);
			//		}
			//	}
			//}
		}

		public void AddKeys(MapManager manager)
		{
			EntityManager entityManager = Locator.Get<EntityManager>();
			TileMap<Cell> map = manager.Map;
			NodeList<Zone> zoneNodeList = map.Layout.Zones.Nodes;
			Zone[] zoneList = new Zone[zoneNodeList.Count - 1];

			int index = 0;

			for (int i = 0; i < zoneNodeList.Count; i++)
			{
				if (zoneNodeList[i].Value != map.Layout.FinalZone)
				{
					zoneList[index++] = zoneNodeList[i].Value;
				}
			}

			Zone keyZone = zoneList[Random.Range(0, zoneList.Length)];
			bool positionFound = false;

			while (!positionFound)
			{
				int tileSelected = Random.Range(0, keyZone.tiles.Count);
				int tileCount = 0;
				var tileEnumerator = keyZone.GetEnumerator();

				while (!positionFound && tileEnumerator.MoveNext())
				{
					if (tileCount >= tileSelected)
					{
						Cell selectedCell = map[tileEnumerator.Current];

						if 
						(
							selectedCell.TileInfo.TileType == TileType.Floor
							&& selectedCell.Prop == null
							&& selectedCell.Items.Count == 0
						)
						{
							Entity key = entityManager.Spawn("Key Silver");
							manager.AddItem(key, tileEnumerator.Current);

							positionFound = true;
						}
					}

					tileCount++;
				}
			}
		}

		public void AddItems(MapManager manager)
		{
			// TODO: actually add items
		}

		private Direction GetDoorDirection(int2 doorPosition, int2 nextTile)
		{
			Direction result;

			if (nextTile.y > doorPosition.y)
			{
				result = Direction.North;
			}
			else if (nextTile.x > doorPosition.x)
			{
				result = Direction.East;
			}
			else if (nextTile.y < doorPosition.y)
			{
				result = Direction.South;
			}
			else
			{
				result = Direction.West;
			}

			return result;
		}
	}
}
