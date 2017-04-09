public interface IZoneGenerator
{
	TileMap<TileType> PreConnectZones(TileMap<TileType> map);
	TileMap<TileType> Generate(TileMap<TileType> map);
	TileMap<TileType> PostConnectZones(TileMap<TileType> map);
}
