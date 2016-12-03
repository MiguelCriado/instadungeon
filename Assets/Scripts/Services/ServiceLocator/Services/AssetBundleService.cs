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
		public abstract void GetAllAssets<T>(string bundleId, UnityAction<T[]> onAssetsLoaded) where T : UnityEngine.Object;
		public abstract void GetAsset<T>(string bundleId, string assetName, UnityAction<T> onAssetLoaded) where T : UnityEngine.Object;
		public abstract void GetAssetAsync<T>(string bundleId, string assetName, UnityAction<T> onAssetLoaded) where T : UnityEngine.Object;
	}
}
