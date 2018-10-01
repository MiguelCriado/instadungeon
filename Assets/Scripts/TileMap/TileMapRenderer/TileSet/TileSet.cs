using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TileSet
{
	public string tilesetName;
	public Texture2D texture;
	public int2 tileResolution;
	public int spacing;
	public int margin;

	private Tile[] tiles;
	private Dictionary<string, AutoTile> autoTiles;

	public TileSet(string name, Texture2D texture, int2 tileResolution) 
		: this(name, texture, tileResolution, 0, 0)
	{
	}

	public TileSet(string name, string texturePath, int2 tileResolution, int spacing, int margin)
	{
		Texture2D texture = Resources.Load<Texture2D>(Path.ChangeExtension(texturePath, null));
		Setup(name, texture, tileResolution, this.spacing, margin);
	}

	public TileSet(string name, Texture2D texture, int2 tileResolution, int spacing, int margin)
	{
		Setup(name, texture, tileResolution, spacing, margin);
	}

	private void Setup(string name, Texture2D texture, int2 tileResolution, int spacing, int margin)
	{
		tilesetName = name;
		this.texture = texture;
		this.tileResolution = tileResolution;
		this.spacing = spacing;
		this.margin = margin;
		autoTiles = new Dictionary<string, AutoTile>();

		int horizontalTileCount = ((texture.width - margin * 2) + spacing) / (tileResolution.x + spacing);
		int verticalTileCount = ((texture.height - margin * 2) + spacing) / (tileResolution.y + spacing);
		int numTiles = horizontalTileCount * verticalTileCount;

		Vector2 percentTileResolution = new Vector2(
			(float)tileResolution.x / texture.width,
			(float)tileResolution.y / texture.height);

		tiles = new Tile[numTiles];

		uint id = 0;

		for (int y = verticalTileCount - 1; y >= 0; y--)
		{
			for (int x = 0; x < horizontalTileCount; x++)
			{
				Rect uvRect = new Rect(
					x * percentTileResolution.x + margin + spacing * x,
					y * percentTileResolution.y + margin + spacing * y,
					percentTileResolution.x,
					percentTileResolution.y);

				Rect rect = new Rect(Vector2.zero, Vector2.one);

				Tile tile = new Tile(id, new Rect[] { rect }, new Rect[] { uvRect });
				tiles[id] = tile;
				id++;
			}
		}
	}

	public void AddAutoTile(AutoTile autoTile)
	{
		autoTiles.Add(autoTile.Id, autoTile);
	}

	public Tile GetTile(uint id)
	{
		return tiles[id];
	}

	public Tile GetTile(string autoTileId, byte mask)
	{
		Tile result = null;
		AutoTile autoTile;

		if (autoTiles.TryGetValue(autoTileId, out autoTile))
		{
			result = autoTile.GetTile(mask);
		}

		return result;
	}
}
