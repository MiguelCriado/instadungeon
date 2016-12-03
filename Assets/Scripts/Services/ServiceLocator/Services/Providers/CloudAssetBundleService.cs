using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace InstaDungeon.Services
{
	public class CloudAssetBundleService : AssetBundleService
	{
		#region Private Type Definitions

		private delegate bool CachedAssetDelegate(out object cachedAsset);

		private class DummyMonoBehaviour : MonoBehaviour { }

		private class AssetBundleDownloadJob
		{
			public AssetBundleInfo bundleInfo;
			public CachedAssetDelegate checkIfAssetIsAlreadyCached;
			public UnityAction<object> onAssetLoaded;
			public UnityAction<AssetBundle> onBundleLoaded;
			public UnityAction<AssetBundleDownloadJob> onJobFinished;

			public AssetBundleDownloadJob(
				AssetBundleInfo bundleInfo,
				CachedAssetDelegate checkIfAssetIsAlreadyCached,
				UnityAction<object> onAssetLoaded,
				UnityAction<AssetBundle> onBundleLoaded,
				UnityAction<AssetBundleDownloadJob> onJobFinished)
			{
				this.bundleInfo = bundleInfo;
				this.checkIfAssetIsAlreadyCached = checkIfAssetIsAlreadyCached;
				this.onAssetLoaded = onAssetLoaded;
				this.onBundleLoaded = onBundleLoaded;
				this.onJobFinished = onJobFinished;
			}

			public void Download()
			{
				object cachedAsset = null;

				if (checkIfAssetIsAlreadyCached(out cachedAsset))
				{
					if (onAssetLoaded != null)
					{
						onAssetLoaded(cachedAsset);
					}

					if (onJobFinished != null)
					{
						onJobFinished.Invoke(this);
					}
				}
				else
				{
					AssetBundleManager.DownloadAssetBundle(bundleInfo.url, bundleInfo.version, OnBundleLoaded);
				}
			}

			private void OnBundleLoaded(AssetBundle bundle)
			{
				if (onBundleLoaded != null)
				{
					onBundleLoaded.Invoke(bundle);
				}

				if (onJobFinished != null)
				{
					onJobFinished.Invoke(this);
				}
			}
		}

		private struct AssetBundleInfo
		{
			public string bundleId;
			public string url;
			public int version;

			public AssetBundleInfo(string bundleId, string url, int version)
			{
				this.bundleId = bundleId;
				this.url = url;
				this.version = version;
			}
		}

		#endregion

		private Dictionary<string, AssetBundleInfo> assetBundles;
		private Dictionary<string, Dictionary<string, WeakReference>> cachedAssets;
		private Dictionary<string, Queue<AssetBundleDownloadJob>> assetBundleDownloadQueue;

		private MonoBehaviour monoBehaviourHelper;
		private bool dirty;
		private string[] assetBundleIdList;

		public CloudAssetBundleService()
		{
			assetBundles = new Dictionary<string, AssetBundleInfo>();
			cachedAssets = new Dictionary<string, Dictionary<string, WeakReference>>();
			assetBundleDownloadQueue = new Dictionary<string, Queue<AssetBundleDownloadJob>>();

			GameObject go = new GameObject("CloudAssetBundleService");
			GameObject.DontDestroyOnLoad(go);
			go.hideFlags |= HideFlags.HideInHierarchy;
			monoBehaviourHelper = go.AddComponent<DummyMonoBehaviour>();
		}

		public override void RegisterBundle(string id, string url, int version)
		{
			if (!assetBundles.ContainsKey(id))
			{
				AssetBundleInfo abInfo = new AssetBundleInfo(id, url, version);
				assetBundles.Add(id, abInfo);
				cachedAssets.Add(id, new Dictionary<string, WeakReference>());
				assetBundleDownloadQueue.Add(id, new Queue<AssetBundleDownloadJob>());

				dirty = true;

				if (OnBundleRegistered != null)
				{
					OnBundleRegistered.Invoke(id);
				}
			}
			else
			{
				if (version > assetBundles[id].version)
				{
					AssetBundleInfo abInfo = new AssetBundleInfo(id, url, version);
					assetBundles.Add(id, abInfo);
					cachedAssets[id] = new Dictionary<string, WeakReference>();

					if (OnBundleRegistered != null)
					{
						OnBundleRegistered.Invoke(id);
					}
				}
			}
		}

		public override bool IsRegistered(string bundleId)
		{
			return assetBundles.ContainsKey(bundleId);
		}

		public override string[] GetRegisteredBundles()
		{
			if (dirty)
			{
				assetBundleIdList = new string[assetBundles.Count];
				int i = 0;

				foreach (var key in assetBundles.Keys)
				{
					assetBundleIdList[i] = key;
					i++;
				}

				dirty = false;
			}

			return assetBundleIdList;
		}

		public override void GetAllAssets<T>(string bundleId, UnityAction<T[]> onAssetsLoaded)
		{
			if (assetBundles.ContainsKey(bundleId))
			{
				UnityAction<AssetBundle> onBundleDownloaded = (AssetBundle bundle) => 
				{
					if (bundle != null)
					{
						T[] result = bundle.LoadAllAssets<T>();

						var cachedBundle = cachedAssets[bundleId];
						string assetName;

						for (int i = 0; i < result.Length; i++)
						{
							cachedBundle = cachedAssets[bundleId];
							assetName = result[i].name;

							if (cachedBundle.ContainsKey(assetName))
							{
								cachedBundle[assetName].Target = result[i];
							}
							else
							{
								cachedBundle.Add(assetName, new WeakReference(result[i]));
							}
						}

						AssetBundleManager.Unload(assetBundles[bundleId].url, assetBundles[bundleId].version, false);

						if (onAssetsLoaded != null)
						{
							onAssetsLoaded(result);
						}
					}
					else
					{
						onAssetsLoaded(default(T[]));
					}
				};

				CachedAssetDelegate checkIfAssetAlreadyCached = (out object cachedAsset) =>
				{
					cachedAsset = null;
					return false;
				};

				EnqueueDownload(new AssetBundleDownloadJob(
						assetBundles[bundleId],
						checkIfAssetAlreadyCached,
						null,
						onBundleDownloaded,
						OnDownloadJobFinished));
			}
			else
			{
				if (onAssetsLoaded != null)
				{
					onAssetsLoaded(default(T[]));
				}
			}

		}

		public override void GetAsset<T>(string bundleId, string assetName, UnityAction<T> onAssetLoaded)
		{
			if (!string.IsNullOrEmpty(assetName))
			{
				LoadAsset(bundleId, assetName, onAssetLoaded, (AssetBundle bundle) =>
				{
					if (bundle != null)
					{
						T result = bundle.LoadAsset<T>(assetName);

						cachedAssets[bundleId].Add(assetName, new WeakReference(result));

						AssetBundleManager.Unload(assetBundles[bundleId].url, assetBundles[bundleId].version, false);

						if (onAssetLoaded != null)
						{
							onAssetLoaded(result);
						}
					}
					else
					{
						onAssetLoaded(default(T));
					}
				});
			}
			else
			{
				Locator.Log.Warning("The assetName cannot be null or empty.");

				if (onAssetLoaded != null)
				{
					onAssetLoaded(default(T));
				}
			}
		}

		public override void GetAssetAsync<T>(string bundleId, string assetName, UnityAction<T> onAssetLoaded)
		{
			if (string.IsNullOrEmpty(assetName))
			{
				LoadAsset(bundleId, assetName, onAssetLoaded, (AssetBundle bundle) =>
				{
					if (bundle != null)
					{
						AssetBundleRequest request = bundle.LoadAssetAsync<T>(assetName);
						monoBehaviourHelper.StartCoroutine(LoadAssetAsync(bundleId, assetName, bundle, request, onAssetLoaded));
					}
					else
					{
						if (onAssetLoaded != null)
						{
							onAssetLoaded(default(T));
						}
					}
				});
			}
			else
			{
				Locator.Log.Warning("The assetName cannot be null or empty.");

				if (onAssetLoaded != null)
				{
					onAssetLoaded(default(T));
				}
			}
		}

		private void LoadAsset<T>(string bundleId, string assetName, UnityAction<T> onAssetLoaded, UnityAction<AssetBundle> onBundleLoaded) where T : UnityEngine.Object
		{
			if (cachedAssets.ContainsKey(bundleId))
			{

				CachedAssetDelegate checkIfAssetAlreadyCached = (out object cachedAsset) =>
				{
					bool result = false;
					cachedAsset = null;

					if (TryGetCachedAsset<T>(bundleId, assetName, out cachedAsset))
					{
						result = true;
					}

					return result;
				};

				UnityAction<object> onAssetRetrieved = (object assetRetrieved) =>
				{
					T asset = assetRetrieved as T;

					if (onAssetLoaded != null)
					{
						onAssetLoaded(asset);
					}
				};

				object cached = null;

				if (!checkIfAssetAlreadyCached(out cached))
				{
					EnqueueDownload(new AssetBundleDownloadJob(
						assetBundles[bundleId],
						checkIfAssetAlreadyCached,
						onAssetRetrieved,
						onBundleLoaded,
						OnDownloadJobFinished));
				}
				else
				{
					onAssetRetrieved(cached);
				}
			}
			else
			{
				Debug.LogWarning("bundleId not found, you must register the bundle before trying to get any asset from it.");

				if (onAssetLoaded != null)
				{
					onAssetLoaded(default(T));
				}
			}
		}

		private IEnumerator LoadAssetAsync<T>(string bundleId, string assetName, AssetBundle bundle, AssetBundleRequest request, UnityAction<T> onAssetLoaded) where T : UnityEngine.Object
		{
			yield return request;

			T asset = request.asset as T;

			cachedAssets[bundleId].Add(assetName, new WeakReference(asset));

			AssetBundleManager.Unload(assetBundles[bundleId].url, assetBundles[bundleId].version, false);

			if (onAssetLoaded != null)
			{
				onAssetLoaded(asset);
			}
		}

		private bool TryGetCachedAsset<T>(string bundleId, string assetName, out object cachedAsset)  where T : UnityEngine.Object
		{
			bool result = false;
			cachedAsset = null;

			if (cachedAssets.ContainsKey(bundleId))
			{
				WeakReference assetReference = null;

				if (cachedAssets[bundleId].TryGetValue(assetName, out assetReference))
				{
					T asset = assetReference.Target as T;

					if (asset != null)
					{
						cachedAsset = asset;
						result = true;
					}
					else
					{
						cachedAssets[bundleId].Remove(assetName);
					}
				}
			}

			return result;
		}

		private void EnqueueDownload(AssetBundleDownloadJob job)
		{
			if (assetBundleDownloadQueue.ContainsKey(job.bundleInfo.bundleId))
			{
				Queue<AssetBundleDownloadJob> queue = assetBundleDownloadQueue[job.bundleInfo.bundleId];
				queue.Enqueue(job);

				if (queue.Count == 1)
				{
					queue.Peek().Download();
				}
			}
		}

		private void OnDownloadJobFinished(AssetBundleDownloadJob job)
		{
			if (assetBundleDownloadQueue.ContainsKey(job.bundleInfo.bundleId))
			{
				Queue<AssetBundleDownloadJob> queue = assetBundleDownloadQueue[job.bundleInfo.bundleId];
				queue.Dequeue();

				if (queue.Count > 0)
				{
					queue.Peek().Download();
				}
			}
		}
	}
}
