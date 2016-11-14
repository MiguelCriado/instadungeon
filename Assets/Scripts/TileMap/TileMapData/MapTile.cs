public enum TileType
{
	Space,
	Floor,
	Wall,
	Door, 
	Entrance, 
	Exit
}

public class MapTile
{
	public TileType TileType { get; private set; }

	public MapTile(TileType type)
	{
		TileType = type;
	}
}
