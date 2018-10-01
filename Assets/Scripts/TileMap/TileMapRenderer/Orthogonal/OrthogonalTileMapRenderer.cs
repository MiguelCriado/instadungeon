using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace InstaDungeon.TileMap
{
	[RequireComponent(typeof(TileSetBehaviour))]
	public class OrthogonalTileMapRenderer : MonoBehaviour, ITileMapRenderer
	{
		public float TileScale { get { return tileScale; } }
		public int2 ChunkSize { get { return chunkSize; } }
		public List<Material> Materials { get { return materials; } }

		[SerializeField] private float tileScale = 1f;
		[SerializeField] private int2 chunkSize;
		[SerializeField] private List<Material> materials;
		[Header("Debug")]
		[SerializeField] private bool drawTileNumbers;
		[SerializeField] private bool ignoreVisibility;


		private Dictionary<int2, OrthogonalChunkRenderer> chunks;
		private Transform chunksContainer;
		private Transform chunkPool;
		private Transform chunkLayerPool;

		private void Awake()
		{
			chunks = new Dictionary<int2, OrthogonalChunkRenderer>();

			GameObject go = new GameObject("Chunks");
			chunksContainer = go.transform;
			chunksContainer.SetParent(transform);

			go = new GameObject("ChunkPool");
			chunkPool = go.transform;
			chunkPool.SetParent(transform);

			go = new GameObject("ChunkLayerPool");
			chunkLayerPool = go.transform;
			chunkLayerPool.SetParent(transform);
		}

#if UNITY_EDITOR
		private MapManager mapManager;

		private void Start()
		{
			mapManager = Locator.Get<MapManager>();
		}

		private void OnValidate()
		{
			if (Application.isPlaying)
			{
				RefreshVisibility();
			}
		}

		private void OnDrawGizmos()
		{
			if (mapManager != null && drawTileNumbers)
			{
				int2[] tiles = mapManager.Map.GetPresentTiles();

				for (int i = 0; i < tiles.Length; i++)
				{
					Handles.Label(TileMapToWorldPosition(tiles[i]), tiles[i].ToString());
				}
			}
		}
#endif

		public void RenderMap(TileMap<Cell> map)
		{
			RecycleChunks(chunks);

			int2[] tiles = map.GetPresentTiles();

			TileSetBehaviour tileSetBehaviour = GetComponent<TileSetBehaviour>();
			TileSet tileSet = tileSetBehaviour.Tileset;

			int2 chunkId;
			OrthogonalChunkRenderer renderer;

			for (int i = 0; i < tiles.Length; i++)
			{
				chunkId = GetChunkId(tiles[i]);

				if (!chunks.TryGetValue(chunkId, out renderer))
				{
					renderer = SpawnRenderer();
					renderer.transform.SetParent(chunksContainer);
					renderer.name = string.Format("Chunk [{0}, {1}]", chunkId.x, chunkId.y);
					renderer.Setup(this, tileSet, materials, map);

					chunks.Add(chunkId, renderer);
				}

				renderer.AddTile(tiles[i]);
			}

			foreach(var chunkRenderer in chunks)
			{
				chunkRenderer.Value.Commit();
			}
		}

		public void RefreshVisibility()
		{
			var enumerator = chunks.GetEnumerator();

			while (enumerator.MoveNext())
			{
				enumerator.Current.Value.RefreshVisibility(ignoreVisibility);
			}
		}

		public Vector3 SnappedTileMapToWorldPosition(int2 tileMapPosition)
		{
			return (tileMapPosition * tileScale) + Vector2.one * (tileScale / 2f);
		}

		public Vector3 TileMapToWorldPosition(Vector2 tileMapPosition)
		{
			return tileMapPosition * tileScale + Vector2.one * 0.5f;
		}

		public int2 WorldToTileMapPosition(Vector3 worldPosition)
		{
			Vector3 scaledPosition = worldPosition / tileScale;
			int2 result = new int2(Mathf.FloorToInt(scaledPosition.x), Mathf.FloorToInt(scaledPosition.y));
			return result;
		}

		public OrthogonalChunkRendererLayer SpawnRendererLayer()
		{
			return SpawnComponentFromPool<OrthogonalChunkRendererLayer>("ChunkLayer", chunkLayerPool);
		}

		private int2 GetChunkId(int2 tilePosition)
		{
			return new int2(tilePosition.x / chunkSize.x, tilePosition.y / chunkSize.y);
		}

		private OrthogonalChunkRenderer SpawnRenderer()
		{
			return SpawnComponentFromPool<OrthogonalChunkRenderer>("Chunk", chunkPool);
		}

		private void RecycleChunks(Dictionary<int2, OrthogonalChunkRenderer> chunkList)
		{
			var enumerator = chunkList.Values.GetEnumerator();

			while (enumerator.MoveNext())
			{
				RecycleComponents(enumerator.Current.DisposeLayers().GetEnumerator(), chunkLayerPool);
				enumerator.Current.transform.SetParent(chunkPool);
				enumerator.Current.gameObject.SetActive(false);
			}

			chunkList.Clear();
		}

		private T SpawnComponentFromPool<T>(string objectName, Transform pool) where T : Component
		{
			T result = null;

			if (pool.childCount > 0)
			{
				Transform child = pool.GetChild(0);
				result = child.GetComponent<T>();

				if (result != null)
				{
					result.gameObject.SetActive(true);
					result.transform.SetParent(null);
				}
			}
			else
			{
				GameObject go = new GameObject(objectName);
				result = go.AddComponent<T>();
			}

			return result;
		}

		private void RecycleComponents<T>(IEnumerator<T> list, Transform destinyPool) where T : Component
		{
			while (list.MoveNext())
			{
				list.Current.transform.SetParent(destinyPool);
				list.Current.gameObject.SetActive(false);
			}
		}
	}
}
