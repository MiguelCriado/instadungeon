using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;

namespace InstaDungeon.Services
{
	static public class AssetBundleManager
	{
		private class DummyMonoBehaviour : MonoBehaviour { }

		static private Dictionary<string, AssetBundleRef> assetBundleRefs;
		static private DummyMonoBehaviour monoBehaviourHelper;

		static AssetBundleManager ()
		{
			assetBundleRefs = new Dictionary<string, AssetBundleRef>();
			GameObject go = new GameObject("AssetBundleManager");
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

		public static void DownloadAssetBundle(string url, int version, UnityAction<AssetBundle> onBundleLoaded)
		{
			monoBehaviourHelper.StartCoroutine(DownloadAssetBundleInternal(url, version, onBundleLoaded));
		}

		public static void DownloadAssetBundle(string url, int version, UnityAction<AssetBundle> onBundleLoaded, UnityAction<float> downloadProgress)
		{
			monoBehaviourHelper.StartCoroutine(DownloadAssetBundleInternal(url, version, onBundleLoaded, downloadProgress));
		}

		private static IEnumerator DownloadAssetBundleInternal(string url, int version, UnityAction<AssetBundle> onBundleLoaded, UnityAction<float> downloadProgress = null)
		{
			string keyName = url + version.ToString();

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

					if (www.error != null)
					{
						Debug.LogError("WWW download:" + www.error);
					}
					else
					{
						AssetBundleRef abRef = new AssetBundleRef(url, version);
						abRef.assetBundle = www.assetBundle;
						assetBundleRefs.Add(keyName, abRef);
					}
				}
			}

			if (onBundleLoaded != null)
			{
				if (assetBundleRefs.ContainsKey(keyName))
				{
					onBundleLoaded(assetBundleRefs[keyName].assetBundle);
				}
				else
				{
					onBundleLoaded(null);
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
