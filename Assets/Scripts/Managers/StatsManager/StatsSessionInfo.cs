using System;
using System.Collections.Generic;

namespace InstaDungeon
{
	public class StatsSessionInfo
	{
		public DateTime StartTime { get; private set; }
		public DateTime EndTime { get; private set; }
		public List<MapStats> StatList { get; private set; }

		public StatsSessionInfo()
		{
			StatList = new List<MapStats>();
		}

		public void StartSession()
		{
			StartTime = DateTime.UtcNow;
			StatList.Clear();
		}

		public void EndSession()
		{
			EndTime = DateTime.UtcNow;
		}

		public void AddStats(MapStats stats)
		{
			StatList.Add(stats);
		}
	}
}
