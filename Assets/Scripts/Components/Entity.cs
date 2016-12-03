using InstaDungeon.Events;
using UnityEngine;

namespace InstaDungeon
{
	public class Entity : MonoBehaviour
	{
		public Blackboard Blackboard { get { return blackboard; } }
		public EventSystem Events { get { return events; } }

		private Blackboard blackboard;
		private EventSystem events;

		void Awake()
		{
			blackboard = new Blackboard();
			events = new EventSystem();
		}

		void Update()
		{
			events.TickUpdate();
		}
	}
}
