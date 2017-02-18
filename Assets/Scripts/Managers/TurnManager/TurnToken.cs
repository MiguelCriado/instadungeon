using InstaDungeon.Actions;
using InstaDungeon.Components;
using UnityEngine;
using UnityEngine.Events;

namespace InstaDungeon
{
	public class TurnToken
	{
		public int Round { get; set; }
		public int Turn { get; set; }
		public bool EntityHasControl { get { return !actionRunning && turnManagerHasControl; } }
		public TurnComponent Target { get; set; }
		public UnityEvent OnActionFinished = new UnityEvent();

		protected bool actionRunning;
		protected bool turnManagerHasControl;
		protected IAction bufferedAction;

		public TurnToken()
		{
			actionRunning = false;
			turnManagerHasControl = true;
			Round = 0;
			Turn = 0;
		}

		public void BufferAction(IAction action)
		{
			bufferedAction = action;
		}

		public void GrantControl()
		{
			turnManagerHasControl = true;
		}

		public void RevokeControl()
		{
			turnManagerHasControl = false;
			bufferedAction = null;
		}

		public void RunBufferedAction()
		{
			if (bufferedAction != null)
			{
				if (EntityHasControl)
				{
					actionRunning = true;
					bufferedAction.OnActionDone.AddListener(OnActionDone);
					bufferedAction.Act();
				}

				bufferedAction = null;
			}
		}

		protected void OnActionDone(IAction action)
		{
			action.OnActionDone.RemoveListener(OnActionDone);
			actionRunning = false;

			if (OnActionFinished != null)
			{
				OnActionFinished.Invoke();
			}
		}
	}
}
