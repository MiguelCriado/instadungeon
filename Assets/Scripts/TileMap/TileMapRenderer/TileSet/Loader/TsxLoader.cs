using System.Collections.Generic;
using Tiled;

public static class TsxLoader
{
	public static TileSet LoadTileset(string filePath)
	{
		TileSet result = null;
		TileSetNode tileSet = null;

		tileSet = TsxParser.ParseTileset(filePath);

		if (tileSet != null)
		{
			result = ConvertTilesetNode(tileSet);
		}

		return result;
	}

	private static TileSet ConvertTilesetNode(TileSetNode tileSet)
	{
		TileSet result = new TileSet(
			tileSet.Name,
			tileSet.Image.Source, 
			new int2(tileSet.TileWidth, tileSet.TileHeight), 
			tileSet.Spacing, 
			tileSet.Margin);

		Dictionary<string, AutoTile> autoTiles = new Dictionary<string, AutoTile>();

		PropertyNode autoTileGroupNode;
		PropertyNode autoTileMaskNode;
		TileNode tile;

		for (int i = 0; i < tileSet.Tiles.Count; i++)
		{
			tile = tileSet.Tiles[i];

			if (tile.Properties != null)
			{
				autoTileGroupNode = tile.Properties.PropertyList.Find(x => x.Name == "autotilegroup");

				if (autoTileGroupNode != null)
				{
					autoTileMaskNode = tile.Properties.PropertyList.Find(x => x.Name == "autotilemask");

					if (autoTileMaskNode != null)
					{
						string autoTileName = autoTileGroupNode.Value as string;

						if (!autoTiles.ContainsKey(autoTileName))
						{
							autoTiles.Add(autoTileName, new AutoTile(autoTileName));
						}

						byte autoTileMask;

						if (autoTileMaskNode.Value.GetType() == typeof(string)
							&& byte.TryParse((string)autoTileMaskNode.Value, out autoTileMask))
						{
							autoTiles[autoTileName].AddTile(autoTileMask, result.GetTile(tile.Id));
						}
					}
				}
			}
		}

		foreach(var autoTile in autoTiles)
		{
			result.AddAutoTile(autoTile.Value);
		}

		return result;
	}
}
