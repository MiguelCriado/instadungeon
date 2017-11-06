public interface IChunkRenderer
{
	void AddTile(int2 tilePosition);
	void Commit();
}
