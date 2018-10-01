using UnityEngine;

namespace InstaDungeon
{
	public class GameLauncher : MonoBehaviour
	{
		[SerializeField] private ControlMode controlMode;

		private void Start()
		{
			Locator.Get<StatsManager>();
			GameSettings settings = Locator.Get<GameFeederManager>().Settings;

			if (settings != null)
			{
				Locator.Get<GameManager>().Initialize(settings.LayoutGenerator, settings.ZoneGenerator, settings.Seed, settings.NumLevels, settings.ControlMode);
			}
			else
			{
				Locator.Get<GameManager>().Initialize(controlMode);
			}
		}
	}
}
