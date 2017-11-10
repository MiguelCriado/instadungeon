using System.Collections.Generic;
using UnityEngine;

public interface ITileMapRenderer
{
	float TileScale { get; }
	int2 ChunkSize { get; }
	List<Material> Materials { get; }

	void RenderMap(TileMap<Cell> map);
	void RefreshVisibility();
	Vector3 SnappedTileMapToWorldPosition(int2 tileMapPosition);
	Vector3 TileMapToWorldPosition(Vector2 tileMapPosition);
	int2 WorldToTileMapPosition(Vector3 worldPosition);
}
