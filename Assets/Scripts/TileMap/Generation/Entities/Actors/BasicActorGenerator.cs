using InstaDungeon.Components;
using Random = UnityEngine.Random;

namespace InstaDungeon
{
	public class BasicActorGenerator : IActorGenerator
	{
		public void AddEnemies(MapManager manager)
		{
			EntityManager entityManager = Locator.Get<EntityManager>();
			TileMap<Cell> map = manager.Map;

			NodeList<Zone> zoneNodeList = map.Layout.Zones.Nodes;
			Zone[] zoneList = new Zone[zoneNodeList.Count - 1];

			int index = 0;

			for (int i = 0; i < zoneNodeList.Count; i++)
			{
				if (zoneNodeList[i].Value != map.Layout.InitialZone)
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
							&& selectedCell.Actor == null
							&& selectedCell.Prop == null
							&& selectedCell.Items.Count == 0
						)
						{
							Entity enemy = entityManager.Spawn("Green Slime");
							manager.AddActor(enemy, tileEnumerator.Current);

							positionFound = true;
						}
					}

					tileCount++;
				}
			}
		}
	}
}
