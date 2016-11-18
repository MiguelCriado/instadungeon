using System.Collections.Generic;
using UnityEngine;

namespace InstaDungeon.TileMap
{
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class OrthogonalChunkRenderer : MonoBehaviour, IChunkRenderer
	{
		private OrthogonalTileMapRenderer tileMapRenderer;
		private TileSet tileSet;
		private Material material;
		private MeshFilter meshFilter;
		private MeshRenderer meshRenderer;

		private TileMap<Cell> map;
		private List<Vector3> vertices;
		private List<Vector3> normals;
		private List<Vector2> uv;
		private List<int> triangles;

		private int vertexOffset;
		float tileHeight;
		float tileWidth;

		void Awake()
		{
			meshFilter = GetComponent<MeshFilter>();
			meshRenderer = GetComponent<MeshRenderer>();
		}

		public void Setup(OrthogonalTileMapRenderer renderer, TileSet tileSet, Material material)
		{
			tileMapRenderer = renderer;
			this.tileSet = tileSet;
			this.material = material;
		}

		public void BeginBuilding(TileMap<Cell> map)
		{
			this.map = map;
			vertices = new List<Vector3>();
			normals = new List<Vector3>();
			uv = new List<Vector2>();
			triangles = new List<int>();

			vertexOffset = 0;
			tileHeight = tileMapRenderer.TileScale;
			tileWidth = tileMapRenderer.TileScale * tileSet.tileResolution.x / tileSet.tileResolution.y;
		}

		public void AddTile(int2 tilePosition)
		{
			int x = tilePosition.x;
			int y = tilePosition.y;

			Tile tile = TileMapper.GetTile(map, tilePosition, tileSet);

			if (tile != null)
			{
				Rect[] rects = tile.Rects;
				Rect[] uvRects = tile.UvRects;

				for (int j = 0; j < rects.Length; j++)
				{
					float xSection = x + (rects[j].x);
					float ySection = y + (rects[j].y);

					float widthSection = rects[j].width;
					float heightSection = rects[j].height;

					vertices.Add(new Vector3(xSection * tileWidth, ySection * tileHeight, 0f));
					vertices.Add(new Vector3(xSection * tileWidth + widthSection, ySection * tileHeight, 0f));
					vertices.Add(new Vector3(xSection * tileWidth, ySection * tileHeight + heightSection, 0f));
					vertices.Add(new Vector3(xSection * tileWidth + widthSection, ySection * tileHeight + heightSection, 0f));

					normals.Add(Vector3.forward);
					normals.Add(Vector3.forward);
					normals.Add(Vector3.forward);
					normals.Add(Vector3.forward);

					uv.Add(uvRects[j].min);
					uv.Add(new Vector2(uvRects[j].xMax, uvRects[j].y));
					uv.Add(new Vector2(uvRects[j].x, uvRects[j].yMax));
					uv.Add(uvRects[j].max);

					triangles.Add(vertexOffset + 0);
					triangles.Add(vertexOffset + 2);
					triangles.Add(vertexOffset + 3);

					triangles.Add(vertexOffset + 0);
					triangles.Add(vertexOffset + 3);
					triangles.Add(vertexOffset + 1);

					vertexOffset += 4;
				}
			}
		}

		public void FinishBuilding()
		{
			Mesh mesh = new Mesh();
			mesh.vertices = vertices.ToArray();
			mesh.normals = normals.ToArray();
			mesh.uv = uv.ToArray();
			mesh.triangles = triangles.ToArray();

			meshFilter.mesh = mesh;

			Material[] rendererMaterials = meshRenderer.materials;
			rendererMaterials[0] = material;
			rendererMaterials[0].mainTexture = tileSet.texture;
			meshRenderer.materials = rendererMaterials;
		}
	}
}
