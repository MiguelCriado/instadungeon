public interface IChunkRenderer
{
	void BeginBuilding(TileMap<Cell> map);
	void AddTile(int2 tilePosition);
	void FinishBuilding();
}
