using UnityEngine;

namespace InstaDungeon.TileMap
{
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class TileMapRenderer : MonoBehaviour
	{
		public int width = 10;
		public int height = 10;
		public float tileSize = 1f;
		public int tileResolution = 8;

		#region MonoBehaviour Methods

		void Start()
		{
			Build();
		}

		#endregion

		[ContextMenu("Build")]
		public void Build()
		{
			BuildMesh();
			GenerateTexture();
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
