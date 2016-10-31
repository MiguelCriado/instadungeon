namespace Tiled
{
	public enum DrawOrder
	{
		TopDown,
		Index
	}

	public class ObjectGroupNode : TmxNode
	{
		public string Name;
		public string Color;
		public float Opacity;
		public bool Visible;
		public int OffsetX;
		public int OffsetY;
		public DrawOrder DrawOrder;
	}
}
