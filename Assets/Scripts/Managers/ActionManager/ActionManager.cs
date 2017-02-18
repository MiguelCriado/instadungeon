using InstaDungeon.Actions;
using InstaDungeon.UnityEvents;
using System.Collections.Generic;
using UnityEngine;

namespace InstaDungeon
{
	public class ActionManager : Manager
	{
		protected class UpdateHelper : MonoBehaviour
		{
			public FloatUnityEvent OnUpdate = new FloatUnityEvent();

			protected void Update()
			{
				if (OnUpdate != null)
				{
					OnUpdate.Invoke(Time.deltaTime);
				}
			}
		}

		protected UpdateHelper updateHelper;
		protected List<IAction> runningActions;

		public ActionManager() : base()
		{
			GetHelper().OnUpdate.AddListener(OnUpdate);
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

		protected void OnUpdate(float deltaTime)
		{
			CommitFinishedActions();
			UpdateActions(deltaTime);
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

		protected UpdateHelper GetHelper()
		{
			if (updateHelper == null)
			{
				updateHelper = monoBehaviourHelper.gameObject.AddComponent<UpdateHelper>();
			}

			return updateHelper;
		}
	}
}
