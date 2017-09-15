namespace InstaDungeon.MapGeneration
{
	public abstract class BaseZoneGenerator<T1, T2> : IZoneGenerator<T1, T2>
		where T1 : IZoneGeneratorSettings<T2>
		where T2 : IZoneLevelSettings
	{
		protected T1 settings;

		public BaseZoneGenerator(T1 settings)
		{
			SetSettings(settings);
		}

		public void SetSettings(T1 settings)
		{
			this.settings = settings;
		}

		public abstract TileMap<TileType> PreConnectZones(TileMap<TileType> map, int level);
		public abstract TileMap<TileType> Generate(TileMap<TileType> map, int level);
		public abstract TileMap<TileType> PostConnectZones(TileMap<TileType> map, int level);
		public abstract int2 GetSpawnPoint(TileMap<TileType> map, int level);
		public abstract int2 GetExitPoint(TileMap<TileType> map, int level);
	}
}
