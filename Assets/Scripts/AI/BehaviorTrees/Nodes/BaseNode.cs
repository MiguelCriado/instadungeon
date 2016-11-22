using System;
using UnityEngine;

namespace AI.BehaviorTrees
{
	public enum NodeStates
	{
		Success,
		Failure,
		Running,
		Error
	}

	public abstract class BaseNode
	{
		private static readonly string IS_OPEN = "isOpen";

		public string Id { get; private set; }

		public BaseNode()
		{
			Id = Guid.NewGuid().ToString();
		}

		public NodeStates Execute(Tick tick)
		{
			NodeStates result;

			EnterWrapper(tick);

			bool isOpen;
			
			if (!tick.Blackboard.TryGet(IS_OPEN, tick.Tree.Id, Id, out isOpen) || isOpen == false)
			{
				OpenWrapper(tick);
			}

			result = TickWrapper(tick);

			if (result != NodeStates.Running)
			{
				CloseWrapper(tick);
			}

			ExitWrapper(tick);

			return result;
		}

		protected virtual void Enter(Tick tick) { }
		protected virtual void Open(Tick tick) { }
		protected virtual NodeStates Tick(Tick tick) { return NodeStates.Success; }
		protected virtual void Close(Tick tick) { }
		protected virtual void Exit(Tick tick) { }

		private void EnterWrapper(Tick tick)
		{
			tick.EnterNode(this);
			Enter(tick);
		}

		private void OpenWrapper(Tick tick)
		{
			tick.OpenNode(this);
			string isOpen = IS_OPEN;
			string treeId = tick.Tree.Id;
			string nodeId = Id;
			Profiler.BeginSample("Jauri fuera!!!!");
			tick.Blackboard.Set(IS_OPEN, true, tick.Tree.Id, Id);
			Profiler.EndSample();
			Open(tick);
		}

		private NodeStates TickWrapper(Tick tick)
		{
			tick.TickNode(this);
			return Tick(tick);
		}

		public void CloseWrapper(Tick tick)
		{
			tick.CloseNode(this);
			tick.Blackboard.Set(IS_OPEN, false, tick.Tree.Id, Id);
			Close(tick);
		}

		private void ExitWrapper(Tick tick)
		{
			tick.ExitNode(this);
			Exit(tick);
		}
	}
}
