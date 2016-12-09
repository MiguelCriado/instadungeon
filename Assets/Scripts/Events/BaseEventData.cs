using UnityEngine;

namespace InstaDungeon.Events
{
	public abstract class BaseEventData : IEventData
	{
		public abstract uint EventType { get; }
		public abstract string Name { get; }
		public float TimeStamp { get; protected set; }

		public BaseEventData()
		{
			TimeStamp = Time.realtimeSinceStartup;
		}

		public abstract IEventData Copy();
	}
}
