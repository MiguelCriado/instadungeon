using InstaDungeon.DataStructures;
using InstaDungeon.Events;
using UnityEngine;

namespace InstaDungeon.Components
{
	[RequireComponent(typeof(CellTransform))]
	public class Entity : MonoBehaviour
	{
		public uint Guid { get { return guid; } }
		public Blackboard Blackboard { get { return blackboard; } }
		public EventSystem Events { get { return events; } }
		public CellTransform CellTransform { get { return cellTransform; } }

		private Blackboard blackboard;
		private EventSystem events;
		private CellTransform cellTransform;
		private uint guid;

		void Awake()
		{
			blackboard = new Blackboard();
			events = new EventSystem();
			cellTransform = GetComponent<CellTransform>();
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
