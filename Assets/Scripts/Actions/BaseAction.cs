using System;
using InstaDungeon.UnityEvents;

namespace InstaDungeon.Actions
{
	public abstract class BaseAction : IAction
	{
		public virtual bool IsActing { get; private set; }
		public virtual bool IsDone { get; private set; }
		public ActionUnityEvent OnActionDone { get { return onActionDone; } }

		protected ActionUnityEvent onActionDone = new ActionUnityEvent();

		public virtual void Act()
		{
			IsActing = true;
			Locator.Get<ActionManager>().AddAction(this);
		}

		public virtual void Update(float deltaTime) { }

		public abstract void Commit();
		
		protected void ActionDone()
		{
			IsActing = false;
			IsDone = true;
		}

		protected void TryToNotify()
		{
			if (onActionDone != null)
			{
				onActionDone.Invoke(this);
			}
		}
	}

	public abstract class BaseAction<T> : BaseAction, IAction<T> where T : Command
	{
		public T Command { get { return command; } }

		protected T command;
		protected CommandManager commandManager;

		public BaseAction()
		{
			commandManager = Locator.Get<CommandManager>();
		}

		public override void Commit()
		{
			commandManager.RegisterCommand(Command);
			TryToNotify();
		}
	}
}
