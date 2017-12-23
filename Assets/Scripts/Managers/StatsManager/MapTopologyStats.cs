namespace InstaDungeon
{
	public class MapTopologyStats
	{
		public RectangleInt MapBounds;
		public float FillRate;
		public int NumZones;
		public RectangleInt BiggestZone;
		public RectangleInt SmallestZone;
		public float BiggestZoneRatio;
		public float MeanZoneConnectionsNumber;
		public int DeadEndedZones;
	}
}
