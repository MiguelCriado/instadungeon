public interface IZoneGenerator
{
	TileMap<TileType> PreConnectZones(TileMap<TileType> map);
	TileMap<TileType> Generate(TileMap<TileType> map);
	TileMap<TileType> PostConnectZones(TileMap<TileType> map);
	int2 PlaceStairs(Zone zone, TileMap<TileType> map);
}
