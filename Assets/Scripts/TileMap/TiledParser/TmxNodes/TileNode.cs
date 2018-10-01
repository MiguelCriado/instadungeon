namespace Tiled
{
	public class TileNode : TmxNode
	{
		public uint Id;
		public int[] Terrain;
		public float Probability;

		public PropertiesNode Properties;
		public ImageNode Image;
		public ObjectGroupNode ObjectGroup;
		public AnimationNode Animation;
	}
}
