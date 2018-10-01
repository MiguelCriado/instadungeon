public enum TileType
{
	Space,
	Floor,
	Wall
}

public class TileInfo
{
	public TileType TileType { get; private set; }
	public bool Walkable { get; private set; }
	public bool BreaksLineOfSight { get; private set; }

	public TileInfo(TileType type, bool walkable)
	{
		TileType = type;
		Walkable = walkable;

		BreaksLineOfSight = type == TileType.Wall;
	}
}
