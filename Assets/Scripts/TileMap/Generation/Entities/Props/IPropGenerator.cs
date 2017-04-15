namespace InstaDungeon
{
	public interface IPropGenerator
	{
		void AddStairs(MapManager manager);
		void AddDoors(MapManager manager);
		void AddKeys(MapManager manager);
		void AddItems(MapManager manager);
	}
}
