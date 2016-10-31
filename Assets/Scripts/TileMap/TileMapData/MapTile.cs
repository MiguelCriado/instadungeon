public enum TileType
{
	Space,
	Floor,
	Wall,
	Door
}

public class MapTile
{
	public TileType TileType { get; private set; }

	public MapTile(TileType type)
	{
		TileType = type;
	}
}
