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
			Locator.Provide<ActionManager>(() =>
			{
				return new ActionManager();
			});

			Locator.Provide<EntityManager>(() =>
			{
				return new EntityManager();
			});

			Locator.Provide<MapManager>(() =>
			{
				return new MapManager();
			});

			Locator.Provide<CommandManager>(() =>
			{
				return new CommandManager();
			});

			Locator.Provide<VisibilityManager>(() =>
			{
				return new VisibilityManager();
			});

			Locator.Provide<TurnManager>(() => 
			{
				return new TurnManager();
			});

			Locator.Provide<ParticleSystemManager>(() =>
			{
				return new ParticleSystemManager();
			});

			Locator.Provide<CameraManager>(() =>
			{
				return new CameraManager();
			});

			Locator.Provide<MapGenerationManager>(() => 
			{
				return new MapGenerationManager();
			});

			Locator.Provide<GameManager>(() => 
			{
				return new GameManager();
			});

			Locator.Provide<SideManager>(() => 
			{
				return new SideManager();
			});

			Locator.Provide<GameFeederManager>(() => 
			{
				return new GameFeederManager();
			});
		}
	}
}
