public enum TileType
{
	Space,
	Floor,
	Wall,
	Door
}

public class Tile
{
	public TileType TileType { get { return tileType; } }

	private TileType tileType;

	public Tile(TileType type)
	{
		tileType = type;
	}
}
