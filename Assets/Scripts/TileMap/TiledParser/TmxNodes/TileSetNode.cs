using System.Collections.Generic;

namespace Tiled
{
	public class TileSetNode : TmxNode
	{
		public string FirstGuid;
		public string Source;
		public string Name;
		public int TileWidth;
		public int TileHeight;
		public int Spacing;
		public int Margin;
		public int TileCount;
		public int Columns;

		public TileOffsetNode TileOffset;
		public PropertiesNode Properties;
		public ImageNode Image;
		public TerrainTypesNode TerrainTypes;
		public List<TileNode> Tiles;
	}
}
