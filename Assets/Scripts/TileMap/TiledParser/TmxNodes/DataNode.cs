using System.Collections.Generic;

namespace Tiled
{
	public enum Encoding
	{
		Base64,
		Csv
	}

	public enum Compression
	{
		Gzip,
		Zlib
	}

	public class DataNode : TmxNode
	{
		public Encoding Encoding;
		public Compression Compression;
		public string Data;

		public List<Tile> tileList;
	}
}
