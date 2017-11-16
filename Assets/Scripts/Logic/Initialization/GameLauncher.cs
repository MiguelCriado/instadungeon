using UnityEngine;

namespace InstaDungeon
{
	public class GameLauncher : MonoBehaviour
	{
		private void Start()
		{
			GameSettings settings = Locator.Get<GameFeederManager>().Settings;

			if (settings != null)
			{
				Locator.Get<GameManager>().Initialize(settings.LayoutGenerator, settings.ZoneGenerator, settings.Seed);
			}
			else
			{
				Locator.Get<GameManager>().Initialize();
			}
		}
	}
}
