﻿using System.Collections.Generic;
using UnityEngine;

namespace InstaDungeon.TileMap
{
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(TileSetBehaviour))]
	public class OrthogonalTileMapRenderer : MonoBehaviour
	{
		public float tileScale = 1f;

		public void BuildMesh(TileMap<MapTile> map)
		{
			int2[] tiles = map.GetPresentTiles();

			List<Vector3> vertices = new List<Vector3>();
			List<Vector3> normals = new List<Vector3>();
			List<Vector2> uv = new List<Vector2>();

			List<int> triangles = new List<int>();

			TileSetBehaviour tileSetBehaviour = GetComponent<TileSetBehaviour>();
			TileSet tileSet = tileSetBehaviour.Tileset;

			float tileHeight = tileScale;
			float tileWidth = tileScale * tileSet.tileResolution.x / tileSet.tileResolution.y;

			int vertexOffset = 0;

			for (int i = 0; i < tiles.Length; i++)
			{
				int2 tilePosition = tiles[i];
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

			Mesh mesh = new Mesh();
			mesh.vertices = vertices.ToArray();
			mesh.normals = normals.ToArray();
			mesh.uv = uv.ToArray();
			mesh.triangles = triangles.ToArray();

			MeshFilter meshFilter = GetComponent<MeshFilter>();
			meshFilter.mesh = mesh;

			MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
			meshRenderer.materials[0].mainTexture = tileSet.texture;
		}
	}
}
