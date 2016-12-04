using RSG;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace InstaDungeon.Services
{
	public class CloudAssetBundleService : AssetBundleService
	{
		#region Private Type Definitions

		private class DummyMonoBehaviour : MonoBehaviour { }

		private class AssetBundleEvent : UnityEvent<AssetBundle> { }

		private abstract class AssetBundleLoadJob
		{
			public AssetBundleInfo BundleInfo { get; protected set; }
			public string AssetName { get; protected set; }

			public AssetBundleEvent OnBundleLoaded = new AssetBundleEvent();
			public abstract void Resolve(object asset);
			public abstract void Reject(Exception ex);
			public abstract void Load();
			public abstract object TryGetCachedAsset();
		}

		private class AssetBundleLoadJob<T> : AssetBundleLoadJob where T : UnityEngine.Object
		{
			private Action<T> resolve;
			private Action<Exception> reject;
			private Action<AssetBundleLoadJob> onLoadFinished;
			private Func<object> tryGetCachedAsset;

			public AssetBundleLoadJob(
				AssetBundleInfo bundleInfo,
				string assetName,
				Action<T> resolve,
				Action<Exception> reject,
				Func<object> tryGetCachedAsset,
				Action<AssetBundleLoadJob> onLoadFinished)
			{
				BundleInfo = bundleInfo;
				AssetName = assetName;
				this.resolve = resolve;
				this.reject = reject;
				this.tryGetCachedAsset = tryGetCachedAsset;
				this.onLoadFinished = onLoadFinished;
			}

			public override void Resolve(object asset)
			{
				Assert.IsNotNull(resolve);
				Assert.IsTrue(asset is T);
				resolve(asset as T);
			}

			public override void Reject(Exception ex)
			{
				Assert.IsNotNull(reject);
				reject(ex);
			}

			public override void Load()
			{
				AssetBundleLoader.LoadAssetBundle(BundleInfo.url, BundleInfo.version)
					.Then(assetBundle =>
					{
						T result = assetBundle.LoadAsset<T>(AssetName);

						if (result != null)
						{
							Assert.IsNotNull(resolve);
							resolve.Invoke(result);

							if (OnBundleLoaded != null)
							{
								OnBundleLoaded.Invoke(assetBundle);
							}
						}
						else
						{
							Assert.IsNotNull(reject);
							reject(new ApplicationException(string.Format("Asset \"{0}\" not found in asset bundle \"{1}\"", AssetName, BundleInfo.url)));
						}

						if (onLoadFinished != null)
						{
							onLoadFinished(this);
						}
					})
					.Catch(ex =>
					{
						Assert.IsNotNull(reject);
						reject(ex);

						if (onLoadFinished != null)
						{
							onLoadFinished(this);
						}
					});
			}

			public override object TryGetCachedAsset()
			{
				if (tryGetCachedAsset != null)
				{
					return TryGetCachedAsset();
				}
				else
				{
					return null;
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
		private Dictionary<string, Queue<AssetBundleLoadJob>> assetBundleDownloadQueue;

		private MonoBehaviour monoBehaviourHelper;
		private bool dirty;
		private string[] assetBundleIdList;

		public CloudAssetBundleService()
		{
			assetBundles = new Dictionary<string, AssetBundleInfo>();
			cachedAssets = new Dictionary<string, Dictionary<string, WeakReference>>();
			assetBundleDownloadQueue = new Dictionary<string, Queue<AssetBundleLoadJob>>();

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
				assetBundleDownloadQueue.Add(id, new Queue<AssetBundleLoadJob>());

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

		public override IPromise<T[]> GetAllAssets<T>(string bundleId)
		{
			AssetBundleInfo bundleInfo;

			if (assetBundles.TryGetValue(bundleId, out bundleInfo))
			{
				return new Promise<T[]>((resolve, reject) => 
				{
					LoadAllAssets(bundleInfo, resolve, reject);
				});
			}
			else
			{
				return new Promise<T[]>((resolve, reject) => 
				{
					reject(new ApplicationException(string.Format("Asset Bundle \"{0}\" not found.", bundleId)));
				});
			}
		}

		public override IPromise<T> GetAsset<T>(string bundleId, string assetName)
		{
			if (!string.IsNullOrEmpty(assetName))
			{
				return LoadAsset<T>(bundleId, assetName);
			}
			else
			{
				return new Promise<T>((resolve, reject) => 
				{
					reject(new ApplicationException("The assetName cannot be null or empty."));
				});
			}
		}

		private IPromise<T> LoadAsset<T>(string bundleId, string assetName) where T : UnityEngine.Object
		{
			if (cachedAssets.ContainsKey(bundleId))
			{
				object cachedAsset = null;

				if (TryGetCachedAsset<T>(bundleId, assetName, out cachedAsset))
				{
					return OnAssetRetrieved(bundleId, assetName, cachedAsset as T);
				}
				else
				{
					return EnqueueDownload<T>(bundleId, assetName);
				}
			}
			else
			{
				return new Promise<T>((resolve, reject) => 
				{
					reject(new ApplicationException("bundleId not found, you must register the bundle before trying to get any asset from it."));
				});
			}
		}

		private void LoadAllAssets<T>(AssetBundleInfo bundleInfo, Action<T[]> resolve, Action<Exception> reject) where T : UnityEngine.Object
		{
			AssetBundleLoader.LoadAssetBundle(bundleInfo.url, bundleInfo.version)
					.Then(bundle =>
					{
						T[] result = bundle.LoadAllAssets<T>();

						for (int i = 0; i < result.Length; i++)
						{
							AddAssetToCache(bundleInfo.bundleId, result[i].name, result[i]);
						}

						resolve(result);
					})
					.Catch(exception =>
					{
						reject(exception);
					});
		}

		private IPromise<T> OnAssetRetrieved<T>(string bundleId, string assetName, T asset) where T : UnityEngine.Object
		{
			return new Promise<T>((resolve, reject) =>
			{
				resolve(asset);
			});
		}

		private IPromise<T> ProcessBundle<T>(string bundleId, string assetName, AssetBundle bundle) where T : UnityEngine.Object
		{
			if (bundle != null)
			{
				T result = bundle.LoadAsset<T>(assetName);

				AddAssetToCache(bundleId, assetName, result);

				AssetBundleLoader.Unload(assetBundles[bundleId].url, assetBundles[bundleId].version, false);

				return new Promise<T>((resolve, reject) =>
				{
					resolve(result);
				});
			}
			else
			{
				return new Promise<T>((resolve, reject) =>
				{
					reject(new ApplicationException(string.Format("Failed to load asset bundle {0}.", bundleId)));
				});
			}
		}

		private void AddAssetToCache<T>(string bundleId, string assetName, T asset) where T : UnityEngine.Object
		{
			if (!cachedAssets.ContainsKey(bundleId))
			{
				cachedAssets.Add(bundleId, new Dictionary<string, WeakReference>());
			}

			Dictionary<string, WeakReference> cache = cachedAssets[bundleId];

			cache[assetName] = new WeakReference(asset);
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

		private IPromise<T> EnqueueDownload<T>(string bundleId, string assetName) where T : UnityEngine.Object
		{
			if (assetBundleDownloadQueue.ContainsKey(bundleId))
			{
				Func<object> tryGetCachedAsset = () => 
				{
					object cachedAsset;

					if (TryGetCachedAsset<T>(bundleId, assetName, out cachedAsset))
					{
						return cachedAsset;
					}

					return null;
				};

				AssetBundleLoadJob<T> job = null;

				IPromise<T> result = new Promise<T>((resolve, reject) => 
				{
					job = new AssetBundleLoadJob<T>(
						assetBundles[bundleId],
						assetName,
						resolve,
						reject,
						tryGetCachedAsset,
						OnLoadJobFinished);
				});

				EnqueueJob(job);

				return result;
			}
			else
			{
				return new Promise<T>((resolve, reject) => 
				{
					reject(new ApplicationException(string.Format("Bundle id {0} not found.")));
				});
			}
		}

		private void EnqueueJob<T>(AssetBundleLoadJob<T> job) where T : UnityEngine.Object
		{
			Queue<AssetBundleLoadJob> queue = assetBundleDownloadQueue[job.BundleInfo.bundleId];

			queue.Enqueue(job);

			if (queue.Count == 1)
			{
				LoadNextJob(job.BundleInfo.bundleId);
			}
		}

		private void LoadNextJob(string queueBundleId)
		{
			Queue<AssetBundleLoadJob> queue = assetBundleDownloadQueue[queueBundleId];

			if (queue.Count > 0)
			{
				AssetBundleLoadJob job = queue.Dequeue();

				object cachedAsset = job.TryGetCachedAsset();

				if (cachedAsset != null)
				{
					job.Resolve(cachedAssets);
				}
				else
				{
					job.Load();
				}
			}
		}

		private void OnLoadJobFinished(AssetBundleLoadJob job)
		{
			if (assetBundleDownloadQueue.ContainsKey(job.BundleInfo.bundleId))
			{
				LoadNextJob(job.BundleInfo.bundleId);
			}
		}
	}
}
