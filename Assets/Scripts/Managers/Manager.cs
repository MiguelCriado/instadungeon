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
			public UnityEvent OnQuit = new UnityEvent();

			protected void Update()
			{
				if (OnUpdate != null)
				{
					OnUpdate.Invoke();
				}
			}

			protected void OnApplicationQuit()
			{
				if (OnQuit != null)
				{
					OnQuit.Invoke();
				}
			}
		}

		protected GameObject gameObject;
		protected DummyMonoBehaviour monoBehaviourHelper;

		private bool registerForUpdateTicks;
		private bool registerForApplicationQuit;

		public Manager() : this(true, false)
		{
			
		}

		public Manager(bool persistentBetweenScenes, bool registerForUpdateTicks) : this(persistentBetweenScenes, registerForUpdateTicks, false)
		{
			
		}

		public Manager(bool persistentBetweenScenes, bool registerForUpdateTicks, bool registerForApplicationQuit)
		{
			this.registerForUpdateTicks = registerForUpdateTicks;
			this.registerForApplicationQuit = registerForApplicationQuit;
			gameObject = CreateGameObject();

			if (persistentBetweenScenes)
			{
				SceneManager.sceneLoaded += OnSceneLoaded;
			}

			RegisterListeners();
		}

		protected GameObject CreateGameObject()
		{
			GameObject result = new GameObject(GetType().Name);
			monoBehaviourHelper = result.AddComponent<DummyMonoBehaviour>();
			monoBehaviourHelper.Manager = this;
			result.transform.SetParent(ManagerUtils.GetOrCreateManagersParent());
			return result;
		}

		protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			gameObject = CreateGameObject();
			RegisterListeners();
		}

		protected virtual void OnUpdate()
		{

		}

		protected virtual void OnQuit()
		{

		}

		protected Transform GetSceneContainer(params string[] containerPath)
		{
			GameObject pathGO = null;

			if (containerPath.Length > 0)
			{
				pathGO = System.Array.Find(SceneManager.GetActiveScene().GetRootGameObjects(), x => string.Compare(containerPath[0], x.name) == 0);
				
				if (pathGO == null)
				{
					pathGO = new GameObject(containerPath[0]);
				}

				for (int i = 1; i < containerPath.Length; i++)
				{
					Transform childTransform = pathGO.transform.Find(containerPath[i]);

					if (childTransform == null)
					{
						Transform parent = pathGO.transform;

						pathGO = new GameObject(containerPath[i]);
						pathGO.transform.SetParent(parent);
					}
					else
					{
						pathGO = childTransform.gameObject;
					}
				}
			}

			return pathGO.transform;
		}

		private void RegisterListeners()
		{
			if (registerForUpdateTicks)
			{
				monoBehaviourHelper.OnUpdate.AddListener(OnUpdate);
			}

			if (registerForApplicationQuit)
			{
				monoBehaviourHelper.OnQuit.AddListener(OnQuit);
			}
		}
	}
}
