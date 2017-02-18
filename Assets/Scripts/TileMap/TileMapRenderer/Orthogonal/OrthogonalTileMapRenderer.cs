using System.Collections.Generic;
using UnityEngine;

namespace InstaDungeon.TileMap
{
	[RequireComponent(typeof(TileSetBehaviour))]
	public class OrthogonalTileMapRenderer : MonoBehaviour, ITileMapRenderer
	{
		public float TileScale { get { return tileScale; } }
		public int2 ChunkSize { get { return chunkSize; } }
		public Material Material { get { return material; } }

		[SerializeField]
		private float tileScale = 1f;
		[SerializeField]
		private int2 chunkSize;
		[SerializeField]
		private Material material;

		private Dictionary<int2, OrthogonalChunkRenderer> chunks;
		private Transform chunksContainer;
		private Transform chunkPool;

		void Awake()
		{
			chunks = new Dictionary<int2, OrthogonalChunkRenderer>();

			GameObject go = new GameObject("Chunks");
			chunksContainer = go.transform;
			chunksContainer.SetParent(transform);

			go = new GameObject("ChunkPool");
			chunkPool = go.transform;
			chunkPool.SetParent(transform);
		}

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
					renderer.Setup(this, tileSet, material);
					renderer.BeginBuilding(map);

					chunks.Add(chunkId, renderer);
				}

				renderer.AddTile(tiles[i]);
			}

			foreach(var chunkRenderer in chunks)
			{
				chunkRenderer.Value.FinishBuilding();
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

		private int2 GetChunkId(int2 tilePosition)
		{
			return new int2(tilePosition.x / chunkSize.x, tilePosition.y / chunkSize.y);
		}

		private OrthogonalChunkRenderer SpawnRenderer()
		{
			OrthogonalChunkRenderer result = null;

			if (chunkPool.childCount > 0)
			{
				Transform child = chunkPool.GetChild(0);
				result = child.GetComponent<OrthogonalChunkRenderer>();

				if (result != null)
				{
					result.gameObject.SetActive(true);
					result.transform.SetParent(null);
				}
			}
			else
			{
				GameObject go = new GameObject("Chunk");
				result = go.AddComponent<OrthogonalChunkRenderer>();
			}

			return result;
		}
		
		private void RecycleChunks(Dictionary<int2, OrthogonalChunkRenderer> chunkList)
		{
			foreach (var chunk in chunkList)
			{
				chunk.Value.transform.SetParent(chunkPool);
				chunk.Value.gameObject.SetActive(false);
			}

			chunkList.Clear();
		}
	}
}
