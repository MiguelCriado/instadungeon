using System.Collections.Generic;
using UnityEngine;

namespace InstaDungeon.TileMap
{
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class OrthogonalChunkRendererLayer : MonoBehaviour, IChunkRendererLayer
	{
		private struct TileVertices
		{
			public int2 tile;
			public int numVertices;

			public TileVertices(int2 tile, int numVertices)
			{
				this.tile = tile;
				this.numVertices = numVertices;
			}
		}

		private static readonly Color32 ObscuredColor = new Color32(26, 21, 32, 255);
		private static readonly Color32 PreviouslySeenColor = new Color32(26, 21, 32, 128);
		private static readonly Color32 VisibleColor = new Color32(26, 21, 32, 0);

		private MeshFilter meshFilter;
		private MeshRenderer meshRenderer;

		private TileMap<Cell> map;
		private Texture tilesetTexture;
		private Material material;

		private List<TileVertices> tiles;
		private List<Vector3> vertices;
		private List<Vector3> normals;
		private List<Vector2> uv;
		private List<Color32> vertexColors;
		private List<int> triangles;

		private int vertexOffset;
		private float tileWidth;
		private float tileHeight;
		private Vector2 localOffset;

		private void Awake()
		{
			meshFilter = GetComponent<MeshFilter>();
			meshRenderer = GetComponent<MeshRenderer>();

			tiles = new List<TileVertices>();
			vertices = new List<Vector3>();
			normals = new List<Vector3>();
			uv = new List<Vector2>();
			vertexColors = new List<Color32>();
			triangles = new List<int>();
		}

		public void BeginBuilding
		(
			TileMap<Cell> map,
			Texture tilesetTexture,
			Material material,
			float tileWidth,
			float tileHeight,
			Vector2 localOffset,
			string sortingLayer,
			int sortingOrder
		)
		{
			this.map = map;
			this.tilesetTexture = tilesetTexture;
			this.material = material;

			tiles.Clear();
			vertices.Clear();
			normals.Clear();
			uv.Clear();
			vertexColors.Clear();
			triangles.Clear();

			vertexOffset = 0;
			this.tileWidth = tileWidth;
			this.tileHeight = tileHeight;
			this.localOffset = localOffset;

			meshRenderer.sortingLayerName = sortingLayer;
			meshRenderer.sortingOrder = sortingOrder;
		}

		public void AddTile(int2 tilePosition, Tile tile)
		{
			float x = tilePosition.x + localOffset.x;
			float y = tilePosition.y + localOffset.y;

			Rect[] rects = tile.Rects;
			Rect[] uvRects = tile.UvRects;

			tiles.Add(new TileVertices(tilePosition, rects.Length * 4));

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

				vertexColors.Add(Color.white);
				vertexColors.Add(Color.white);
				vertexColors.Add(Color.white);
				vertexColors.Add(Color.white);

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

		public void Commit()
		{
			Mesh mesh = new Mesh();
			mesh.vertices = vertices.ToArray();
			mesh.normals = normals.ToArray();
			mesh.uv = uv.ToArray();
			mesh.colors32 = vertexColors.ToArray();
			mesh.triangles = triangles.ToArray();

			meshFilter.mesh = mesh;

			Material[] rendererMaterials = meshRenderer.materials;
			rendererMaterials[0] = material;
			rendererMaterials[0].mainTexture = tilesetTexture;
			meshRenderer.materials = rendererMaterials;
		}

		public void RefreshVisibility(bool ignoreVisibility)
		{
			Cell currentCell;
			Color32 cellColor;

			vertexColors.Clear();

			for (int i = 0; i < tiles.Count; i++)
			{
				if (ignoreVisibility)
				{
					cellColor = VisibleColor;
				}
				else
				{
					currentCell = map[tiles[i].tile];

					switch (currentCell.Visibility)
					{
						default:
						case VisibilityType.Obscured: cellColor = ObscuredColor; break;
						case VisibilityType.PreviouslySeen: cellColor = PreviouslySeenColor; break;
						case VisibilityType.Visible: cellColor = VisibleColor; break;
					}
				}

				for (int j = 0; j < tiles[i].numVertices; j++)
				{
					vertexColors.Add(cellColor);
				}
			}

			meshFilter.mesh.colors32 = vertexColors.ToArray();
		}
	}
}
