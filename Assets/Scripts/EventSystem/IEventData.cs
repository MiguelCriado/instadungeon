using EventType = System.UInt32;

namespace InstaDungeon.Events
{
	public interface IEventData
	{
		EventType EventType { get; }
		float TimeStamp { get; }
		string Name { get; }

		IEventData Copy();
	}
}
