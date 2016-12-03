using EventType = System.UInt32;

namespace InstaDungeon.Events
{
	public delegate void EventListener(IEventData eventData);

	public interface IEventSystem
	{
		bool AddListener(EventListener listener, EventType eventType);
		bool RemoveListener(EventListener listener, EventType eventType);

		bool TriggerEvent(IEventData eventData);
		bool QueueEvent(IEventData eventData);
		bool AbortEvent(IEventData eventData, bool allOfType = false);

		bool TickUpdate(int maxMillis = int.MaxValue);
	}
}
