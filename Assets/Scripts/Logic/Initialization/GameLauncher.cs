using UnityEngine;

namespace InstaDungeon
{
	public class GameLauncher : MonoBehaviour
	{
		private void Start()
		{
			Locator.Get<GameManager>().Initialize();
		}
	}
}
