using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using EventType = System.UInt32;

namespace InstaDungeon.Events
{
	public class EventSystem : IEventSystem
	{
		private class EventSystemMonoBehaviour : MonoBehaviour
		{
			public EventSystem System { get { return system; } set { system = value; } }

			private EventSystem system;

			void Update()
			{
				Assert.IsNotNull(system, "The global EventSystem is missing.");

				system.TickUpdate();
			}
		}

		private const int EVENTSYSTEM_NUM_QUEUES = 2;

		public static EventSystem Get
		{
			get
			{
				if (instance != null)
				{
					return instance;
				}
				else
				{
					Locator.Log.Error("You are trying to get the global EventSystem but it's not defined.");
					return null;
				}
			}
		}

		private static EventSystem instance;

		private Dictionary<EventType, List<EventListener>> listeners;
		private Queue<IEventData>[] eventQueues;
		private int activeQueue;
		private EventSystemMonoBehaviour monoBehaviourHelper;

		public EventSystem(bool setAsGlobal = false)
		{
			listeners = new Dictionary<EventType, List<EventListener>>();
			eventQueues = new Queue<IEventData>[EVENTSYSTEM_NUM_QUEUES];
			activeQueue = 0;

			for (int i = 0; i < EVENTSYSTEM_NUM_QUEUES; i++)
			{
				eventQueues[i] = new Queue<IEventData>();
			}

			if (setAsGlobal)
			{
				if (instance != null)
				{
					Locator.Log.Warning("You are declaring an EventSystem as global but it's been already defined. The last one will be overriden.");
				}

				monoBehaviourHelper = GetHelperObject();
				monoBehaviourHelper.System = this;
				instance = this;
			}
		}

		public bool AddListener(EventListener listener, EventType eventType)
		{
			if (!listeners.ContainsKey(eventType))
			{
				listeners.Add(eventType, new List<EventListener>());
			}

			if (listeners[eventType].Contains(listener))
			{
				Locator.Log.Warning("Attempting to double-register a delegate");
				return false;
			}
			else
			{
				listeners[eventType].Add(listener);
				return true;
			}
		}

		public bool RemoveListener(EventListener listener, EventType eventType)
		{
			bool success = false;
			List<EventListener> listenerList;

			if (listeners.TryGetValue(eventType, out listenerList))
			{
				success = listenerList.Remove(listener);
			}

			return success;
		}

		public bool TriggerEvent(IEventData eventData)
		{
			bool processed = false;

			List<EventListener> listenerList;

			if (listeners.TryGetValue(eventData.EventType, out listenerList))
			{
				for (int i = 0; i < listenerList.Count; i++)
				{
					listenerList[i].Invoke(eventData);
					processed = true;
				}
			}

			return processed;
		}

		public bool QueueEvent(IEventData eventData)
		{
			Assert.IsTrue(activeQueue >= 0);
			Assert.IsTrue(activeQueue < EVENTSYSTEM_NUM_QUEUES);

			if (listeners.ContainsKey(eventData.EventType))
			{
				eventQueues[activeQueue].Enqueue(eventData);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool AbortEvent(IEventData eventData, bool allOfType = false)
		{
			Assert.IsTrue(activeQueue >= 0);
			Assert.IsTrue(activeQueue < EVENTSYSTEM_NUM_QUEUES);

			bool success = false;

			if (listeners.ContainsKey(eventData.EventType))
			{
				Queue<IEventData> queue = eventQueues[activeQueue];
				int queueCount = queue.Count;
				IEventData current;

				for (int i = 0; i < queueCount; i++)
				{
					current = queue.Dequeue();

					if (current.EventType != eventData.EventType
						|| (!allOfType && success))
					{
						queue.Enqueue(current);
					}
					else
					{
						success = true;
					}
				}
			}

			return success;
		}

		public bool TickUpdate(int maxMillis = int.MaxValue)
		{
			int currentMillis = Environment.TickCount;
			int topMillis = maxMillis == int.MaxValue ? maxMillis : currentMillis + maxMillis;

			Queue<IEventData> queueToProcess = eventQueues[activeQueue];
			activeQueue = (activeQueue + 1) % EVENTSYSTEM_NUM_QUEUES;
			eventQueues[activeQueue].Clear();

			while (queueToProcess.Count > 0)
			{
				IEventData currentEvent = queueToProcess.Dequeue();
				List<EventListener> listenersList;

				if (listeners.TryGetValue(currentEvent.EventType, out listenersList))
				{
					for (int i = 0; i < listenersList.Count; i++)
					{
						listenersList[i].Invoke(currentEvent);
					}
				}

				if (maxMillis != int.MaxValue && Environment.TickCount >= topMillis)
				{
					Locator.Log.Info("Aborting event processing; time ran out.");
					break;
				}
			}

			bool queueFlushed = queueToProcess.Count <= 0;

			if (!queueFlushed)
			{
				Queue<IEventData> otherQueue = eventQueues[activeQueue];

				while (otherQueue.Count > 0)
				{
					queueToProcess.Enqueue(otherQueue.Dequeue());
				}

				Queue<IEventData> auxQueue = otherQueue;
				otherQueue = queueToProcess;
				queueToProcess = auxQueue;
			}

			return queueFlushed;
		}

		private EventSystemMonoBehaviour GetHelperObject()
		{
			EventSystemMonoBehaviour result;

			if (instance != null)
			{
				result = instance.monoBehaviourHelper;
			}
			else
			{
				GameObject go = new GameObject("GlobalEventSystem");
				result = go.AddComponent<EventSystemMonoBehaviour>();
				GameObject.DontDestroyOnLoad(go);
			}

			return result;
		}
	}
}
