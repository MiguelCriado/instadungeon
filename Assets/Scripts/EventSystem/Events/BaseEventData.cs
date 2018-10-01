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

		public IEventData Copy()
		{
			BaseEventData result = CopySpecificData();
			result.TimeStamp = TimeStamp;
			return result;
		}

		public abstract BaseEventData CopySpecificData();
	}
}
