namespace InstaDungeon.Events
{
	public class CellVisibilityEventData : BaseEventData
	{
		public static readonly uint EVENT_TYPE = 0xd7c26548;

		public override uint EventType { get { return EVENT_TYPE; } }
		public override string Name { get { return "CellVisibilityEventData"; } }
		public Cell Cell { get; private set; }
		public VisibilityType PreviousVisibility { get; private set; }
		public VisibilityType CurrentVisibility { get; private set; }

		public CellVisibilityEventData(Cell cell, VisibilityType previousVisibility, VisibilityType currentVisibility)
		{
			Cell = cell;
			PreviousVisibility = previousVisibility;
			CurrentVisibility = currentVisibility;
		}

		public override BaseEventData CopySpecificData()
		{
			return new CellVisibilityEventData(Cell, PreviousVisibility, CurrentVisibility);
		}
	}
}
