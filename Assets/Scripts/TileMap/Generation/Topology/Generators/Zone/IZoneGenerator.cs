namespace InstaDungeon.MapGeneration
{
	public interface IZoneGenerator
	{
		TileMap<TileType> PreConnectZones(TileMap<TileType> map, int level);
		TileMap<TileType> Generate(TileMap<TileType> map, int level);
		TileMap<TileType> PostConnectZones(TileMap<TileType> map, int level);
		int2 GetSpawnPoint(TileMap<TileType> map, int level);
		int2 GetExitPoint(TileMap<TileType> map, int level);
	}

	public interface IZoneGenerator<T1, T2> : IZoneGenerator
	where T1 : IZoneGeneratorSettings<T2>
	where T2 : IZoneLevelSettings
	{
		void SetSettings(T1 settings);
	}
}
