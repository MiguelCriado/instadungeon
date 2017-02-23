using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace InstaDungeon
{
	[System.Serializable]
	public abstract class Manager
	{
		[System.Serializable]
		protected class DummyMonoBehaviour : MonoBehaviour
		{
			public Manager Manager { get { return manager; } set { manager = value; } }

			[SerializeField] protected Manager manager;

			public UnityEvent OnUpdate = new UnityEvent();

			protected void Update()
			{
				if (OnUpdate != null)
				{
					OnUpdate.Invoke();
				}
			}
		}

		protected DummyMonoBehaviour monoBehaviourHelper;

		public Manager() : this(true, false)
		{
			
		}

		public Manager(bool persistentBetweenScenes, bool registerForUpdateTicks)
		{
			CreateGameObject();

			if (persistentBetweenScenes)
			{
				SceneManager.sceneUnloaded += OnSceneUnLoaded;
			}

			if (registerForUpdateTicks)
			{
				monoBehaviourHelper.OnUpdate.AddListener(OnUpdate);
			}
		}

		protected void CreateGameObject()
		{
			GameObject go = new GameObject(GetType().Name);
			monoBehaviourHelper = go.AddComponent<DummyMonoBehaviour>();
			monoBehaviourHelper.Manager = this;
			go.transform.SetParent(ManagerUtils.GetOrCreateManagersParent());
		}

		protected virtual void OnSceneUnLoaded(Scene scene)
		{
			CreateGameObject();
		}

		protected virtual void OnUpdate()
		{

		}
	}
}
