using UnityEngine;
using System.Collections.Generic;

public class TileSet : MonoBehaviour
{
	public Texture2D texture;
	public int2 tileResolution;

	private Dictionary<TileType, List<Rect>> tiles;

	#region ToRemove

	void Awake()
	{
		Setup(texture, tileResolution);

		float tileWidth = (float)tileResolution.x / texture.width;
		float tileHeight = (float)tileResolution.y / texture.height;

		SetTile(TileType.Space, new Rect(0, 0, tileWidth, tileHeight));
		SetTile(TileType.Floor, new Rect(8f / 32f, 0, tileWidth, tileHeight));
		SetTile(TileType.Wall, new Rect(16f / 32f, 0, tileWidth, tileHeight));
		SetTile(TileType.Door, new Rect(24f / 32f, 0, tileWidth, tileHeight));
	}

	public void Setup(Texture2D texture, int2 tileResolution)
	{
		this.texture = texture;
		this.tileResolution = tileResolution;
		tiles = new Dictionary<TileType, List<Rect>>();
	}

	#endregion

	public TileSet(Texture2D texture, int2 tileResolution)
	{
		this.texture = texture;
		this.tileResolution = tileResolution;
		tiles = new Dictionary<TileType, List<Rect>>();
	}

	public void SetTile(TileType type, Rect tile)
	{
		if (!tiles.ContainsKey(type))
		{
			tiles.Add(type, new List<Rect>());
		}

		tiles[type].Add(tile);
	}

	public bool GetTile(TileType type, out Rect tile)
	{
		bool result = false;

		if (tiles.ContainsKey(type))
		{
			tile = tiles[type][Random.Range(0, tiles[type].Count)];
			result = true;
		}
		else
		{
			tile = new Rect();
		}

		return result;
	}
}
