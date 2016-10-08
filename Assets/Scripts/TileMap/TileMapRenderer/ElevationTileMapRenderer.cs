using System.Diagnostics;
using UnityEngine;

namespace InstaDungeon.TileMap
{
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(TileSet))]
	public class ElevationTileMapRenderer : MonoBehaviour
	{
		public int width = 10;
		public int height = 10;
		public float tileSize = 1f;
		public int tileResolution = 8;

		#region MonoBehaviour Methods

		void Start()
		{
			NewRandomMap();
		}

		#endregion

		[ContextMenu("New Random Map")]
		public void NewRandomMap()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();

			TileMap<Tile> map = new TileMap<Tile>();

			Tile tile;

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					tile = new Tile((TileType)Random.Range(0, 4));

					if (tile.TileType != TileType.Space)
					{
						map[x, y] = tile;
					}
				}
			}

			BuildMesh(map);

			stopwatch.Stop();

			UnityEngine.Debug.Log("Time elapsed = " + stopwatch.ElapsedMilliseconds);
		}

		[ContextMenu("Build")]
		public void Build()
		{
			BuildMesh();
			GenerateTexture();
		}

		public void BuildMesh(TileMap<Tile> map)
		{
			int2[] tiles = map.GetPresentTiles();

			int tileCount = tiles.Length;
			int numVertices = tileCount * 4;
			int numTriangles = tileCount * 2;

			Vector3[] vertices = new Vector3[numVertices];
			Vector3[] normals = new Vector3[numVertices];
			Vector2[] uv = new Vector2[numVertices];

			int[] triangles = new int[numTriangles * 3];

			TileSet tileSet = GetComponent<TileSet>();

			int vertexOffset = 0;
			int triangleOffset = 0;

			for (int i = 0; i < tiles.Length; i++)
			{
				int2 tilePosition = tiles[i];
				int x = tilePosition.x;
				int y = tilePosition.y;

				Tile tile = map[tilePosition.x, tilePosition.y];

				Rect tileUV;

				if (tileSet.GetTile(tile.TileType, out tileUV))
				{
					vertices[vertexOffset + 0] = new Vector3(x * tileSize, y * tileSize, 0f);
					vertices[vertexOffset + 1] = new Vector3(x * tileSize + tileSize, y * tileSize, 0f);
					vertices[vertexOffset + 2] = new Vector3(x * tileSize, y * tileSize + tileSize, 0f);
					vertices[vertexOffset + 3] = new Vector3(x * tileSize + tileSize, y * tileSize + tileSize, 0f);

					normals[vertexOffset + 0] = Vector3.forward;
					normals[vertexOffset + 1] = Vector3.forward;
					normals[vertexOffset + 2] = Vector3.forward;
					normals[vertexOffset + 3] = Vector3.forward;

					uv[vertexOffset + 0] = tileUV.min;
					uv[vertexOffset + 1] = new Vector2(tileUV.xMax, tileUV.y);
					uv[vertexOffset + 2] = new Vector2(tileUV.x, tileUV.yMax);
					uv[vertexOffset + 3] = tileUV.max;

					triangles[triangleOffset + 0] = vertexOffset + 0;
					triangles[triangleOffset + 1] = vertexOffset + 2;
					triangles[triangleOffset + 2] = vertexOffset + 3;

					triangles[triangleOffset + 3] = vertexOffset + 0;
					triangles[triangleOffset + 4] = vertexOffset + 3;
					triangles[triangleOffset + 5] = vertexOffset + 1;
				}

				vertexOffset += 4;
				triangleOffset += 6;
			}

			Mesh mesh = new Mesh();
			mesh.vertices = vertices;
			mesh.normals = normals;
			mesh.uv = uv;
			mesh.triangles = triangles;

			MeshFilter meshFilter = GetComponent<MeshFilter>();
			meshFilter.mesh = mesh;

			MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
			meshRenderer.materials[0].mainTexture = tileSet.texture;
		}

		[ContextMenu("Build RandomMesh")]
		public void BuildRandomMesh()
		{
			int numVertices = width * height * 4;
			int numTriangles = width * height * 2;

			Vector3[] vertices = new Vector3[numVertices];
			Vector3[] normals = new Vector3[numVertices];
			Vector2[] uv = new Vector2[numVertices];

			int[] triangles = new int[numTriangles * 3];

			int vertexOffset = 0;
			int triangleOffset = 0;

			TileSet tileSet = GetComponent<TileSet>();

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					Rect tileUV;

					if (tileSet.GetTile((TileType)Random.Range(0, 4), out tileUV))
					{
						vertices[vertexOffset + 0] = new Vector3(x * tileSize, y * tileSize, 0f);
						vertices[vertexOffset + 1] = new Vector3(x * tileSize + tileSize, y * tileSize, 0f);
						vertices[vertexOffset + 2] = new Vector3(x * tileSize, y * tileSize + tileSize, 0f);
						vertices[vertexOffset + 3] = new Vector3(x * tileSize + tileSize, y * tileSize + tileSize, 0f);

						normals[vertexOffset + 0] = Vector3.forward;
						normals[vertexOffset + 1] = Vector3.forward;
						normals[vertexOffset + 2] = Vector3.forward;
						normals[vertexOffset + 3] = Vector3.forward;

						uv[vertexOffset + 0] = tileUV.min;
						uv[vertexOffset + 1] = new Vector2(tileUV.xMax, tileUV.y);
						uv[vertexOffset + 2] = new Vector2(tileUV.x, tileUV.yMax);
						uv[vertexOffset + 3] = tileUV.max;

						triangles[triangleOffset + 0] = vertexOffset + 0;
						triangles[triangleOffset + 1] = vertexOffset + 2;
						triangles[triangleOffset + 2] = vertexOffset + 3;

						triangles[triangleOffset + 3] = vertexOffset + 0;
						triangles[triangleOffset + 4] = vertexOffset + 3;
						triangles[triangleOffset + 5] = vertexOffset + 1;
					}

					vertexOffset += 4;
					triangleOffset += 6;
				}
			}

			Mesh mesh = new Mesh();
			mesh.vertices = vertices;
			mesh.normals = normals;
			mesh.uv = uv;
			mesh.triangles = triangles;

			MeshFilter meshFilter = GetComponent<MeshFilter>();
			meshFilter.mesh = mesh;

			MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
			meshRenderer.materials[0].mainTexture = tileSet.texture;
		}

		[ContextMenu("Build Mesh")]
		public void BuildMesh()
		{
			int sizeX = width + 1;
			int sizeY = height + 1;
			int numVertices = sizeX * sizeY;
			int numTriangles = width * height * 2;

			Vector3[] vertices = new Vector3[numVertices];
			Vector3[] normals = new Vector3[numVertices];
			Vector2[] uv = new Vector2[numVertices];

			int[] triangles = new int[numTriangles * 3];

			for (int y = 0; y < sizeY; y++)
			{
				for (int x = 0; x < sizeX; x++)
				{
					vertices[y * sizeX + x] = new Vector3(x * tileSize, y * tileSize, 0f);
					normals[y * sizeX + x] = new Vector3(0f, 0f, 1f);
					uv[y * sizeX + x] = new Vector2((float)x / width, (float)y / height); 
				}
			}

			int triangleOffset = 0;

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					triangles[triangleOffset + 0] = (y + 1) * sizeX + x;
					triangles[triangleOffset + 1] = y * sizeX + x + 1;
					triangles[triangleOffset + 2] = y * sizeX + x;

					triangles[triangleOffset + 3] = triangles[triangleOffset + 0];
					triangles[triangleOffset + 4] = (y + 1) * sizeX + x + 1;
					triangles[triangleOffset + 5] = triangles[triangleOffset + 1];

					triangleOffset += 6;
				}
			}

			Mesh mesh = new Mesh();
			mesh.vertices = vertices;
			mesh.normals = normals;
			mesh.uv = uv;
			mesh.triangles = triangles;

			MeshFilter meshFilter = GetComponent<MeshFilter>();
			meshFilter.mesh = mesh;
		}

		private void GenerateTexture()
		{
			int textureWidth = width * tileResolution;
			int textureHeight = height * tileResolution;

			Texture2D texture = new Texture2D(textureWidth, textureHeight);
			texture.anisoLevel = 0;
			texture.filterMode = FilterMode.Point;

			for (int y = 0; y < textureHeight; y++)
			{
				for (int x = 0; x < textureWidth; x++)
				{
					Color pixel = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
					texture.SetPixel(x, y, pixel);
				}
			}

			texture.Apply();

			MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
			meshRenderer.materials[0].mainTexture = texture;
		}
	}
}
