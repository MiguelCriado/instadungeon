using RSG;
using UnityEngine.Events;

namespace InstaDungeon.Services
{
	public class AssetBundleEvent : UnityEvent<string> { }

	public abstract class AssetBundleService : Service
	{
		public AssetBundleEvent OnBundleRegistered = new AssetBundleEvent();

		public abstract void RegisterBundle(string id, string url, int version);
		public abstract bool IsRegistered(string bundleId);
		public abstract string[] GetRegisteredBundles();
		public abstract IPromise<T[]> GetAllAssets<T>(string bundleId) where T : UnityEngine.Object;
		public abstract IPromise<T> GetAsset<T>(string bundleId, string assetName) where T : UnityEngine.Object;
	}
}
