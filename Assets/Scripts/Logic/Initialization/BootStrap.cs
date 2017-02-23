using InstaDungeon.Services;
using UnityEngine;
using Logger = InstaDungeon.Services.Logger;

namespace InstaDungeon
{
	public class BootStrap : MonoBehaviour
	{
		public bool overrideServices;

		protected void Awake()
		{
			AddServices();
			AddManagers();
		}

		protected void AddServices()
		{
			if (!Locator.Contains<Logger>() || overrideServices)
			{
				Locator.Provide<Logger>(() =>
				{
					return new ConsoleLogger(LogLevel.Debug);
				});
			}

			if (!Locator.Contains<AssetBundleService>() || overrideServices)
			{
				Locator.Provide<AssetBundleService>(() =>
				{
					return new CloudAssetBundleService();
				});
			}
		}

		protected void AddManagers()
		{
			Locator.Provide<ActionManager>
			(
				() =>
				{
					return new ActionManager();
				}
			);

			Locator.Provide<EntityManager>
			(
				() =>
				{
					return new EntityManager();
				}
			);

			Locator.Provide<MapManager>
			(
				() =>
				{
					return new MapManager();
				}
			);

			Locator.Provide<CommandManager>
			(
				() =>
				{
					return new CommandManager();
				}
			);

			Locator.Provide<VisibilityManager>
			(
				() =>
				{
					return new VisibilityManager();
				}
			);
		}
	}
}
