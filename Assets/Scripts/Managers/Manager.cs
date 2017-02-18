using UnityEngine;
using UnityEngine.SceneManagement;

namespace InstaDungeon
{
	[System.Serializable]
	public abstract class Manager
	{
		protected class DummyMonoBehaviour : MonoBehaviour
		{
			public Manager Manager { get { return manager; } set { manager = value; } }

			[SerializeField] protected Manager manager;
		}

		protected DummyMonoBehaviour monoBehaviourHelper;

		public Manager() : this(true)
		{
			
		}

		public Manager(bool persistentBetweenScenes)
		{
			CreateGameObject();

			if (persistentBetweenScenes)
			{
				SceneManager.sceneUnloaded += OnSceneUnLoaded;
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
	}
}
