using InstaDungeon.DataStructures;
using InstaDungeon.Events;
using UnityEngine;

namespace InstaDungeon.Components
{
	public class Entity : MonoBehaviour
	{
		public uint Guid { get { return guid; } }
		public Blackboard Blackboard { get { return blackboard; } }
		public EventSystem Events { get { return events; } }

		private Blackboard blackboard;
		private EventSystem events;
		private uint guid;

		void Awake()
		{
			blackboard = new Blackboard();
			events = new EventSystem();
		}

		void Update()
		{
			events.TickUpdate();
		}

		public void Init(uint guid)
		{
			this.guid = guid;
		}
	}
}
