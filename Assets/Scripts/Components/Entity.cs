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

		public bool BlocksLineOfSight { get { return blocksLineOfSight; } set { blocksLineOfSight = value; } }
		public bool BlocksMovement { get { return blocksMovement; } set { blocksMovement = value; } }

		[SerializeField] private bool blocksLineOfSight;
		[SerializeField] private bool blocksMovement;

		private Blackboard blackboard;
		private EventSystem events;
		private CellTransform cellTransform;
		private uint guid;

		protected void Reset()
		{
			blocksLineOfSight = false;
			blocksMovement = true;
		}

		protected void Awake()
		{
			blackboard = new Blackboard();
			events = new EventSystem();
			cellTransform = GetComponent<CellTransform>();
		}

		protected void Update()
		{
			events.TickUpdate();
		}

		public void Init(uint guid)
		{
			this.guid = guid;
		}
	}
}
