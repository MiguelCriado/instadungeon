using InstaDungeon.UnityEvents;

namespace InstaDungeon.Actions
{
	public interface IAction
	{
		ActionUnityEvent OnActionDone { get; }
		bool IsActing { get; }
		bool IsDone { get; }

		void Act();
		void Update(float deltaTime);
		void Commit();
	}

	public interface IAction<T> : IAction where T : Command
	{
		T Command { get; }
	}
}
