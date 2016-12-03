using InstaDungeon.Services;
using UnityEngine;
using Logger = InstaDungeon.Services.Logger;

namespace InstaDungeon
{
	public class BootStrap : MonoBehaviour
	{
		public bool overrideServices;

		void Awake()
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
	}
}
