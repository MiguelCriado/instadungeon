using System.IO;
using UnityEngine;

public class TileSetBehaviour : MonoBehaviour
{
	[Header("Direct Mode")]
	public string tilesetName;
	public Texture2D texture;
	public int2 tileResolution;
	public int spacing;
	public int margin;
	[Header("Tiled Mode")]
	public bool loadTileSet = false;
	[Tooltip("Path relative to Resources folder")]
	public string tilesetPath;

	public TileSet Tileset { get { return tileset; } }

	private TileSet tileset;

	void Awake()
	{
		if (loadTileSet && !string.IsNullOrEmpty(tilesetPath))
		{
			string fullPath = Path.Combine(Application.streamingAssetsPath, tilesetPath);
			tileset = TsxLoader.LoadTileset(fullPath);
		}
		else
		{
			tileset = new TileSet(tilesetName, texture, tileResolution, spacing, margin);
		}
	}

	public Tile GetTile(uint id)
	{
		return tileset.GetTile(id);
	}
}
