namespace InstaDungeon.MapGeneration
{
	public class MapGenerator
	{
		public IZoneGenerator ZoneGenerator { get; set; }
		public ILayoutGenerator LayoutGenerator { get; set; }

		public MapGenerator(IZoneGenerator zoneGenerator, ILayoutGenerator layoutGenerator)
		{
			ZoneGenerator = zoneGenerator;
			LayoutGenerator = layoutGenerator;
		}

		#region [Public API]

		public TileMap<Cell> GenerateNewMap(int level, int levelSeed)
		{
			TileMap<TileType> blueprint = GenerateBlueprint(level, levelSeed);
			return GenerateWorld(blueprint, level);
		}

		public TileMap<Cell> GenerateNewMap(int level)
		{
			int seed = System.Guid.NewGuid().GetHashCode() ^ System.DateTime.UtcNow.Millisecond;
			return GenerateNewMap(level, seed);
		}

		#endregion

		#region [Helpers]

		private TileMap<TileType> GenerateBlueprint(int level, int levelSeed)
		{
			TileMap<TileType> result = ZoneConnector.BuildMap(LayoutGenerator, ZoneGenerator, level, levelSeed);
			return result;
		}

		private TileMap<Cell> GenerateWorld(TileMap<TileType> blueprint, int level)
		{
			TileMap<Cell> map = GenerateMap(blueprint);
			MapManager mapManager = Locator.Get<MapManager>();
			mapManager.Initialize(map);
			AddEntities(map, level);
			return map;
		}

		private TileMap<Cell> GenerateMap(TileMap<TileType> blueprint)
		{
			TileMap<Cell> result = blueprint.Convert((TileType cellType) =>
			{
				bool walkable = true;

				if (cellType == TileType.Wall)
				{
					walkable = false;
				}

				return new Cell(new TileInfo(cellType, walkable));
			});

			return result;
		}

		private void AddEntities(TileMap<Cell> map, int level)
		{
			MapManager mapManager = Locator.Get<MapManager>();

			IPropGenerator propGenerator = new BasicPropGenerator();
			propGenerator.AddStairs(mapManager, level);
			propGenerator.AddDoors(mapManager, level);
			propGenerator.AddKeys(mapManager, level);
			propGenerator.AddItems(mapManager, level);

			IActorGenerator actorGenerator = new BasicActorGenerator();
			actorGenerator.AddEnemies(mapManager, level);
		}

		#endregion
	}
}
