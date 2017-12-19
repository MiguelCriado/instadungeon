namespace InstaDungeon
{
	public class MapStats
	{
		public int Level { get; set; }
		public int Seed { get; set; }
		public string LayoutGenerator { get; set; }
		public string ZoneGenerator { get; set; }
		public MapTopologyStats Topology { get; set; }
		public MapNavigationStats Navigation { get; set; }

		public MapStats()
		{
			Topology = new MapTopologyStats();
			Navigation = new MapNavigationStats();
		}
	}
}
