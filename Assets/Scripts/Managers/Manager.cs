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

		protected GameObject gameObject;
		protected DummyMonoBehaviour monoBehaviourHelper;

		public Manager() : this(true, false)
		{
			
		}

		public Manager(bool persistentBetweenScenes, bool registerForUpdateTicks)
		{
			gameObject = CreateGameObject();

			if (persistentBetweenScenes)
			{
				SceneManager.sceneUnloaded += OnSceneUnLoaded;
			}

			if (registerForUpdateTicks)
			{
				monoBehaviourHelper.OnUpdate.AddListener(OnUpdate);
			}
		}

		protected GameObject CreateGameObject()
		{
			GameObject result = new GameObject(GetType().Name);
			monoBehaviourHelper = result.AddComponent<DummyMonoBehaviour>();
			monoBehaviourHelper.Manager = this;
			result.transform.SetParent(ManagerUtils.GetOrCreateManagersParent());
			return result;
		}

		protected virtual void OnSceneUnLoaded(Scene scene)
		{
			gameObject = CreateGameObject();
		}

		protected virtual void OnUpdate()
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
	}
}
