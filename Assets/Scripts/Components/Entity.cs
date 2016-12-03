using UnityEngine;

namespace InstaDungeon
{
	public class Entity : MonoBehaviour
	{
		public Blackboard Blackboard { get { return blackboard; } }

		private Blackboard blackboard;

		void Awake()
		{
			blackboard = new Blackboard();
		}
	}
}
