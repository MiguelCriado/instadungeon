using System.Collections.Generic;
using UnityEngine;

namespace InstaDungeon.TileMap
{
	public class OrthogonalChunkRenderer : MonoBehaviour, IChunkRenderer
	{
		private OrthogonalTileMapRenderer tileMapRenderer;
		private TileSet tileSet;
		private List<Material> materials;
		private TileMap<Cell> map;
		private float tileHeight;
		private float tileWidth;
		private Dictionary<string, OrthogonalChunkRendererLayer> layers;

		private void Awake()
		{
			layers = new Dictionary<string, OrthogonalChunkRendererLayer>();
		}

		public void Setup(OrthogonalTileMapRenderer renderer, TileSet tileSet, List<Material> materials, TileMap<Cell> map)
		{
			tileMapRenderer = renderer;
			this.tileSet = tileSet;
			this.materials = materials;
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

					if (!layers.TryGetValue(layerInfo.SortingLayer, out renderer))
					{
						Vector2 layerOffset = new Vector2(layerInfo.OffsetUnits.x * tileWidth, layerInfo.OffsetUnits.y * tileWidth);
						Material material = materials.Find(x => x.name == layerInfo.Material);

						renderer = tileMapRenderer.SpawnRendererLayer();
						renderer.transform.SetParent(transform);
						renderer.name = string.Format("Layer {0}", layerInfo.SortingLayer);
						renderer.BeginBuilding(map, tileSet.texture, material, tileWidth, tileHeight, layerOffset, layerInfo.SortingLayer, layerInfo.SortingOrder);

						Vector3 newLayerPosition = renderer.transform.localPosition;
						renderer.transform.localPosition = newLayerPosition;

						layers.Add(layerInfo.SortingLayer, renderer);
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
