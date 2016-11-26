public enum TileType
{
	Space,
	Floor,
	Wall,
	Door, 
	Entrance, 
	Exit
}

public class TileInfo
{
	public TileType TileType { get; private set; }
	public bool Walkable { get; private set; }

	public TileInfo(TileType type, bool walkable)
	{
		TileType = type;
		Walkable = walkable;
	}
}
