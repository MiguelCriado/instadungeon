using InstaDungeon.Actions;
using System.Collections.Generic;
using UnityEngine;

namespace InstaDungeon
{
	public class ActionManager : Manager
	{
		protected List<IAction> runningActions;

		public ActionManager() : base(true, true)
		{
			runningActions = new List<IAction>();
		}

		public void AddAction(IAction action)
		{
			runningActions.Add(action);
			
			if (!action.IsActing)
			{
				action.Act();
			}
		}

		protected override void OnUpdate()
		{
			CommitFinishedActions();
			UpdateActions(Time.deltaTime);
		}

		protected void CommitFinishedActions()
		{
			int i = 0;

			while (i < runningActions.Count)
			{
				if (runningActions[i].IsDone)
				{
					runningActions[i].Commit();
					runningActions.RemoveAt(i);
				}
				else
				{
					i++;
				}
			}
		}

		protected void UpdateActions(float deltaTime)
		{
			for (int i = 0; i < runningActions.Count; i++)
			{
				runningActions[i].Update(deltaTime);
			}
		}
	}
}
