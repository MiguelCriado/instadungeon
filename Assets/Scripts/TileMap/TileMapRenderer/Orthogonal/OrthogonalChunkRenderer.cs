using System.Collections.Generic;
using UnityEngine;

namespace InstaDungeon.TileMap
{
	public class OrthogonalChunkRenderer : MonoBehaviour, IChunkRenderer
	{
		private OrthogonalTileMapRenderer tileMapRenderer;
		private TileSet tileSet;
		private Material material;
		private TileMap<Cell> map;
		private float tileHeight;
		private float tileWidth;
		private Dictionary<int, OrthogonalChunkRendererLayer> layers;

		private void Awake()
		{
			layers = new Dictionary<int, OrthogonalChunkRendererLayer>();
		}

		public void Setup(OrthogonalTileMapRenderer renderer, TileSet tileSet, Material material, TileMap<Cell> map)
		{
			tileMapRenderer = renderer;
			this.tileSet = tileSet;
			this.material = material;
			this.map = map;

			tileHeight = tileMapRenderer.TileScale;
			tileWidth = tileMapRenderer.TileScale * tileSet.tileResolution.x / tileSet.tileResolution.y;

			layers.Clear();
		}

		public void AddTile(int2 tilePosition)
		{
			List<TileMapper.TileLayerInfo> tileLayers = TileMapper.GetTileLayers(map, tilePosition, tileSet);
			OrthogonalChunkRendererLayer renderer;
			TileMapper.TileLayerInfo layerInfo;

			if (tileLayers != null)
			{
				for (int i = 0; i < tileLayers.Count; i++)
				{
					layerInfo = tileLayers[i];

					if (!layers.TryGetValue(layerInfo.Layer, out renderer))
					{
						Vector2 layerOffset = Vector2.up * tileHeight * layerInfo.Layer;

						renderer = tileMapRenderer.SpawnRendererLayer();
						renderer.transform.SetParent(transform);
						renderer.name = string.Format("Layer {0}", layerInfo.Layer);
						renderer.BeginBuilding(map, tileSet.texture, material, tileWidth, tileHeight, layerOffset);

						Vector3 newLayerPosition = renderer.transform.localPosition;
						newLayerPosition.z = -1 * layerInfo.Layer;
						renderer.transform.localPosition = newLayerPosition;

						layers.Add(layerInfo.Layer, renderer);
					}

					renderer.AddTile(tilePosition, layerInfo.Tile);
				}
			}
		}

		public void Commit()
		{
			var layerEnumerator = layers.Values.GetEnumerator();

			while (layerEnumerator.MoveNext())
			{
				layerEnumerator.Current.Commit();
			}
		}

		public void RefreshVisibility()
		{
			var layerEnumerator = layers.Values.GetEnumerator();

			while (layerEnumerator.MoveNext())
			{
				layerEnumerator.Current.RefreshVisibility();
			}
		}

		public List<OrthogonalChunkRendererLayer> DisposeLayers()
		{
			List<OrthogonalChunkRendererLayer> result = new List<OrthogonalChunkRendererLayer>();
			var enumerator = layers.Values.GetEnumerator();

			while (enumerator.MoveNext())
			{
				result.Add(enumerator.Current);
			}

			layers.Clear();
			return result;
		}
	}
}
