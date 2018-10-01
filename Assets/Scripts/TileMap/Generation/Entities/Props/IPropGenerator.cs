namespace InstaDungeon
{
	public interface IPropGenerator
	{
		void AddStairs(MapManager manager, int level);
		void AddDoors(MapManager manager, int level);
		void AddKeys(MapManager manager, int level);
		void AddItems(MapManager manager, int level);
	}
}
