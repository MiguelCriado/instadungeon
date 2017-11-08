using UnityEngine;

public interface IChunkRendererLayer 
{
	void BeginBuilding(TileMap<Cell> map, Texture tilesetTexture, Material material, float tileWidth, float tileHeight, Vector2 localOffset, string sortingLayer);
	void AddTile(int2 tilePosition, Tile tile);
	void Commit();
}