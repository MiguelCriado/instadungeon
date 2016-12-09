using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using RSG;
using System;

namespace InstaDungeon.Services
{
	static public class AssetBundleLoader
	{
		private class DummyMonoBehaviour : MonoBehaviour { }

		private static Dictionary<string, AssetBundleRef> assetBundleRefs;
		private static DummyMonoBehaviour monoBehaviourHelper;

		static AssetBundleLoader ()
		{
			assetBundleRefs = new Dictionary<string, AssetBundleRef>();
			GameObject go = new GameObject("AssetBundleLoader");
			go.hideFlags |= HideFlags.HideInHierarchy;
			GameObject.DontDestroyOnLoad(go);
			monoBehaviourHelper = go.AddComponent<DummyMonoBehaviour>();
		}

		private struct AssetBundleRef
		{
			public AssetBundle assetBundle;
			public int version;
			public string url;

			public AssetBundleRef(string strUrlIn, int intVersionIn)
			{
				assetBundle = null;
				url = strUrlIn;
				version = intVersionIn;
			}
		}

		public static AssetBundle GetAssetBundle(string url, int version)
		{
			AssetBundle result = null;
			
			string keyName = url + version.ToString();
			AssetBundleRef abRef;

			if (assetBundleRefs.TryGetValue(keyName, out abRef))
			{
				result = abRef.assetBundle;
			}

			return result;
		}

		public static IPromise<AssetBundle> LoadAssetBundle(string url, int version)
		{
			return LoadAssetBundle(url, version, null);
		}

		public static IPromise<AssetBundle> LoadAssetBundle(string url, int version, UnityAction<float> downloadProgress)
		{
			return new Promise<AssetBundle>((resolve, reject) => 
				monoBehaviourHelper.StartCoroutine(LoadAssetBundleInternal(
					url,
					version,
					resolve,
					reject,
					downloadProgress
				))
			);
		}

		private static IEnumerator LoadAssetBundleInternal(
			string url,
			int version,
			Action<AssetBundle> resolve,
			Action<Exception> reject,
			UnityAction<float> downloadProgress = null)
		{
			string keyName = url + version.ToString();
			bool failed = false;

			if (assetBundleRefs.ContainsKey(keyName))
			{
				yield return null;
			}
			else
			{
				while (!Caching.ready)
				{
					yield return null;
				}

				using (WWW www = WWW.LoadFromCacheOrDownload(url, version))
				{
					if (downloadProgress != null)
					{
						while (!www.isDone)
						{
							downloadProgress(www.progress);
							yield return null;
						}
					}
					else
					{
						yield return www;
					}

					if (!string.IsNullOrEmpty(www.error))
					{
						failed = true;
						reject(new ApplicationException(string.Format("Failed to load asset bundle {0}\r\n{1}", url, www.error)));
					}
					else
					{
						AssetBundleRef abRef = new AssetBundleRef(url, version);
						abRef.assetBundle = www.assetBundle;
						assetBundleRefs.Add(keyName, abRef);
					}
				}
			}

			if (!failed)
			{
				AssetBundleRef result;

				if (assetBundleRefs.TryGetValue(keyName, out result))
				{
					resolve(result.assetBundle);
				}
				else
				{
					reject(new ApplicationException(string.Format("Asset bundle {0} not found in cache.", url)));
				}
			}
		}

		public static void Unload (string url, int version, bool allObjects)
		{
			string keyName = url + version.ToString();
			AssetBundleRef abRef;

			if (assetBundleRefs.TryGetValue(keyName, out abRef))
			{
				abRef.assetBundle.Unload(allObjects);
				abRef.assetBundle = null;
				assetBundleRefs.Remove(keyName);
			}
		}
	}
}
