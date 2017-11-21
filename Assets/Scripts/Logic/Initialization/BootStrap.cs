using InstaDungeon.Services;
using System;
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
			Provide<Logger>(() =>
			{
				return new ConsoleLogger(LogLevel.Debug);
			});

			Provide<AssetBundleService>(() =>
			{
				return new CloudAssetBundleService();
			});
		}

		protected void AddManagers()
		{
			Provide(() =>
			{
				return new ActionManager();
			});

			Provide(() =>
			{
				return new EntityManager();
			});

			Provide(() =>
			{
				return new MapManager();
			});

			Provide(() =>
			{
				return new CommandManager();
			});

			Provide(() =>
			{
				return new VisibilityManager();
			});

			Provide(() => 
			{
				return new TurnManager();
			});

			Provide(() =>
			{
				return new ParticleSystemManager();
			});

			Provide(() =>
			{
				return new CameraManager();
			});

			Provide(() => 
			{
				return new MapGenerationManager();
			});

			Provide(() => 
			{
				return new GameManager();
			});

			Provide(() => 
			{
				return new SideManager();
			});

			Provide(() => 
			{
				return new GameFeederManager();
			});
		}

		private void Provide<T>(Func<T> provider)
		{
			if (!Locator.Contains<T>() || overrideServices)
			{
				Locator.Provide(provider);
			}
		}
	}
}
