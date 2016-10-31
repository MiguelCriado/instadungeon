using System.Collections.Generic;
using System.Xml;

namespace Tiled
{
	public static class TsxParser
	{
		public static TileSetNode ParseTileset(string filePath)
		{
			TileSetNode result = null;

			using (XmlReader reader = XmlReader.Create(filePath))
			{
				result = ParseTileSet(reader);
			}

			return result;
		}

		public static TileSetNode ParseTileSet(XmlReader reader)
		{
			TileSetNode result = null;

			if (reader.ReadToFollowing("tileset"))
			{
				result = ParseNode(reader) as TileSetNode;
			}

			return result;
		}

		private static TmxNode ParseNode(XmlReader reader)
		{
			TmxNode result = null;

			switch (reader.Name)
			{
				case "tileset":
					result = ParseTileSetNode(reader);
					break;
				case "tileoffset":
					// TODO
					break;
				case "image":
					result = ParseImage(reader);
					break;
				case "terraintypes":
					// TODO
					break;
				case "terrain":
					// TODO
					break;
				case "tile":
					result = ParseTile(reader);
					break;
				case "animation":
					// TODO
					break;
				case "frame":
					// TODO
					break;
				case "layer":
					// TODO
					break;
				case "data":
					// TODO
					break;
				case "objectgroup":
					// TODO
					break;
				case "object":
					// TODO
					break;
				case "ellipse":
					// TODO
					break;
				case "polygon":
					// TODO
					break;
				case "polyline":
					// TODO
					break;
				case "imagelayer":
					// TODO
					break;
				case "properties":
					result = ParseProperties(reader);
					break;
				case "property":
					result = ParseProperty(reader);
					break;
			}

			return result;
		}

		#region Parsers

		private static TileSetNode ParseTileSetNode(XmlReader reader)
		{
			TileSetNode tileSet = new TileSetNode();

			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					switch (reader.Name)
					{
						case "firstguid": tileSet.FirstGuid = reader.Value; break;
						case "source": tileSet.Source = reader.Value; break;
						case "name": tileSet.Name = reader.Value; break;
						case "tilewidth": tileSet.TileWidth = reader.ReadContentAsInt(); break;
						case "tileheight": tileSet.TileHeight = reader.ReadContentAsInt(); break;
						case "spacing": tileSet.Spacing = reader.ReadContentAsInt(); break;
						case "margin": tileSet.Margin = reader.ReadContentAsInt(); break;
						case "tilecount": tileSet.TileCount = reader.ReadContentAsInt(); break;
						case "columns": tileSet.Columns = reader.ReadContentAsInt(); break;
					}
				}

				reader.MoveToElement();
			}

			XmlReader inner = reader.ReadSubtree();

			tileSet.Tiles = new List<TileNode>();

			while (inner.Read())
			{
				switch (inner.Name)
				{
					case "tileoffset": tileSet.TileOffset = ParseNode(inner) as TileOffsetNode; break;
					case "properties": tileSet.Properties = ParseNode(inner) as PropertiesNode; break;
					case "image": tileSet.Image = ParseNode(inner) as ImageNode; break;
					case "terraintypes": tileSet.TerrainTypes = ParseNode(inner) as TerrainTypesNode; break;
					case "tile": tileSet.Tiles.Add(ParseNode(inner) as TileNode); break;
				}
			}

			inner.Close();

			return tileSet;
		}

		private static ImageNode ParseImage(XmlReader reader)
		{
			ImageNode result = new ImageNode();

			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					switch (reader.Name)
					{
						case "format": result.Format = reader.Value; break;
						case "source": result.Source = reader.Value; break;
						case "trans": result.Trans = reader.Value; break;
						case "width": result.Width = reader.ReadContentAsInt(); break;
						case "height": result.Height = reader.ReadContentAsInt(); break;
					}
				}

				reader.MoveToElement();
			}

			XmlReader inner = reader.ReadSubtree();

			while (inner.Read())
			{
				switch (inner.Name)
				{
					case "data": result.Data = ParseNode(inner) as DataNode; break;
				}
			}

			inner.Close();

			return result;
		}

		private static TileNode ParseTile(XmlReader reader)
		{
			TileNode result = new TileNode();

			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					switch (reader.Name)
					{
						case "id": result.Id = (uint)reader.ReadContentAsInt(); break;
						case "terrain":
							string value = reader.Value;
							string[] splitted = value.Split(',');
							int[] terrain = new int[splitted.Length];

							for (int i = 0; i < splitted.Length; i++)
							{
								if (!string.IsNullOrEmpty(splitted[i]))
								{
									terrain[i] = int.Parse(splitted[i]);
								}
								else
								{
									terrain[i] = -1;
								}
							}

							result.Terrain = terrain;
							break;
						case "probability": result.Probability = reader.ReadContentAsFloat(); break;
					}
				}

				reader.MoveToElement();
			}

			XmlReader inner = reader.ReadSubtree();

			while (inner.Read())
			{
				switch (inner.Name)
				{
					case "properties": result.Properties = ParseNode(inner) as PropertiesNode; break;
					case "image": result.Image = ParseNode(inner) as ImageNode; break;
					case "objectgroup": result.ObjectGroup = ParseNode(inner) as ObjectGroupNode; break;
					case "animation": result.Animation = ParseNode(inner) as AnimationNode; break;
				}
			}

			inner.Close();

			return result;
		}

		private static PropertiesNode ParseProperties(XmlReader reader)
		{
			PropertiesNode result = new PropertiesNode();

			result.PropertyList = new List<PropertyNode>();

			XmlReader inner = reader.ReadSubtree();

			while (inner.Read())
			{
				switch (inner.Name)
				{
					case "property": result.PropertyList.Add(ParseNode(inner) as PropertyNode); break;
				}
			}

			inner.Close();

			return result;
		}

		private static PropertyNode ParseProperty(XmlReader reader)
		{
			PropertyNode result = new PropertyNode();

			if (reader.HasAttributes)
			{
				while (reader.MoveToNextAttribute())
				{
					switch (reader.Name)
					{
						case "name": result.Name = reader.Value; break;
						case "type":
							switch (reader.Value)
							{
								case "string": result.type = typeof(string); break;
								case "int": result.type = typeof(int); break;
								case "float": result.type = typeof(float); break;
								case "bool": result.type = typeof(bool); break;
							}

							break;
						case "value": result.Value = reader.Value; break;
					}
				}

				reader.MoveToElement();
			}

			return result;
		}

		#endregion
	}
}
