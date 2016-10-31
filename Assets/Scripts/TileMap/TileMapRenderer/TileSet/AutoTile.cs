using System.Collections.Generic;
using UnityEngine;

public class AutoTile
{
	#region Mask values

	// masks are built clockwise, starting from NW corner

	private static readonly byte TOP_CHECK = 64;	// 01000000
	private static readonly byte RIGHT_CHECK = 16;	// 00010000
	private static readonly byte BOTTOM_CHECK = 4;	// 00000100
	private static readonly byte LEFT_CHECK = 1;	// 00000001

	private static readonly byte TOP_MASK = 31;		// 00011111
	private static readonly byte RIGHT_MASK = 199;	// 11000111
	private static readonly byte BOTTOM_MASK = 241;	// 11110001
	private static readonly byte LEFT_MASK = 124;   // 01111100
	#endregion

	#region Autotile Definition Values (Autotile Ids)

	// Ids are built clockwise, starting from NW corner

	#region Visual Reference
	// ·╌╌╌·╌╌╌·╌╌╌·		·╌╌╌·╌╌╌·╌╌╌·
	// ╎┏━┓╎   ╎ ║ ╎		╎000╎101╎010╎
	// ╎┃0┃╎ 1 ╎═╬═╎ - 2	╎0 0╎0 0╎1 1╎
	// ╎┗━┛╎   ╎ ║ ╎		╎000╎101╎010╎
	// ·╌╌╌·╌╌╌·╌╌╌·		·╌╌╌·╌╌╌·╌╌╌·
	// ╎┏━━╎━━━╎━━┓╎		╎000╎000╎000╎
	// ╎┃3 ╎ 4 ╎ 5┃╎		╎0 1╎101╎1 0╎
	// ╎┃  ╎   ╎  ┃╎		╎011╎111╎110╎
	// ·╌╌╌·╌╌╌·╌╌╌·		·╌╌╌·╌╌╌·╌╌╌·
	// ╎┃  ╎   ╎  ┃╎		╎011╎111╎110╎
	// ╎┃6 ╎ 7 ╎ 8┃╎		╎0 1╎1 1╎1 0╎
	// ╎┃  ╎   ╎  ┃╎		╎011╎111╎110╎
	// ·╌╌╌·╌╌╌·╌╌╌·		·╌╌╌·╌╌╌·╌╌╌·
	// ╎┃  ╎   ╎  ┃╎		╎011╎111╎110╎
	// ╎┃9 ╎ A ╎ B┃╎		╎0 1╎101╎1 0╎
	// ╎┗━━╎━━━╎━━┛╎		╎000╎000╎000╎
	// ·╌╌╌·╌╌╌·╌╌╌·		·╌╌╌·╌╌╌·╌╌╌·
	#endregion

	private static readonly byte BLOCK_AUTOTILE = 0;		// (0) 00000000
	private static readonly byte BLANK_AUTOTILE = 170;		// (1) 10101010
	private static readonly byte CROSS_AUTOTILE = 85;		// (2) 01010101
	private static readonly byte NW_CORNER_AUTOTILE = 28;	// (3) 00011100
	private static readonly byte N_AUTOTILE = 31;			// (4) 00011111
	private static readonly byte NE_CORNER_AUTOTILE = 7;	// (5) 00000111
	private static readonly byte W_AUTOTILE = 124;          // (6) 01111100
	private static readonly byte CENTER_AUTOTILE = 255;     // (7) 11111111
	private static readonly byte E_AUTOTILE = 199;          // (8) 11000111
	private static readonly byte SW_CORNER_AUTOTILE = 112;  // (9) 01110000
	private static readonly byte S_AUTOTILE = 241;          // (A) 11110001
	private static readonly byte SE_CORNER_AUTOTILE = 193;  // (B) 11000001

	#endregion

	public string Id { get; private set; }

	private Dictionary<byte, Tile> tiles;
	private Dictionary<byte, Tile> autoTileSet;
	private bool dirty;

	public AutoTile(string id)
	{
		Id = id;
		tiles = new Dictionary<byte, Tile>();
		dirty = true;
	}

	public void AddTile(byte autotileId, Tile tile)
	{
		if (!tiles.ContainsKey(autotileId))
		{
			tiles.Add(autotileId, tile);
			dirty = true;
		}
	}

	public Tile GetTile(byte mask)
	{
		Tile result;

		if (dirty)
		{
			autoTileSet = BuildAutoTileSet();
			dirty = false;
		}

		autoTileSet.TryGetValue(ApplyFilters(mask), out result);

		return result;
	}

	private byte ApplyFilters(byte value)
	{
		byte result = value;

		if ((result & TOP_CHECK) == 0)
		{
			result &= TOP_MASK;
		}

		if ((result & RIGHT_CHECK) == 0)
		{
			result &= RIGHT_MASK;
		}

		if ((result & BOTTOM_CHECK) == 0)
		{
			result &= BOTTOM_MASK;
		}

		if ((result & LEFT_CHECK) == 0)
		{
			result &= LEFT_MASK;
		}

		return result;
	}

	private Dictionary<byte, Tile> BuildAutoTileSet()
	{
		Dictionary<byte, Tile> result = new Dictionary<byte, Tile>();

		byte[] targetMaskList = new byte[]
		{
			255, // 11111111. Check for 0 different neighbours (+1)
			127, // 01111111. Check for 1 different neighbour (+8)
			95,  // 01011111. Check for 2 different neighbours, distance 2 (+8)
			111, // 01101111. Check for 2 different neighbours, distance 3 (+8)
			119, // 01110111. Check for 2 diferent neighbours, distance 4 (+4)
			87,  // 01010111. Check for 3 different neighbours, distance [1, 1] (+8)
			91,  // 01011011. Check for 3 different neighbours, distance [1, 2] (+8)
			85,  // 01010101. Check for 4 different neighbours, distance [1, 1, 1] (+2)
		};

		int[] tileCheckAmount = new int[] { 1, 8, 8, 8, 4, 8, 8, 2};

		byte target;
		byte maskedTarget;
		Tile newTile;

		for (int i = 0; i < targetMaskList.Length; i++)
		{
			target = targetMaskList[i];

			for (int j = 0; j < tileCheckAmount[i]; j++)
			{
				maskedTarget = ApplyFilters(target);

				if (!tiles.TryGetValue(maskedTarget, out newTile))
				{
					newTile = BuildTile(maskedTarget);
				}

				if (newTile != null)
				{
					result.Add(maskedTarget, newTile);
				}

				target = target.RotateRight(1);
			}
		}
		
		return result;
	}

	private Tile BuildTile(byte targetMask)
	{
		Tile result = null;
		List<Rect> rects = new List<Rect>();
		List<Rect> uvRects = new List<Rect>();

		byte closest = FindClosestTile(targetMask, tiles);

		byte currentMask = 193; // 11000001
		byte matchingTile = 0;

		for (int i = 0; i < 4; i++)
		{
			if (MaskedBytesMatch(currentMask, closest, targetMask))
			{
				matchingTile = closest;
			}
			else
			{
				foreach(var tile in tiles)
				{
					if (MaskedBytesMatch(currentMask, tile.Key, targetMask))
					{
						matchingTile = tile.Key;
						break;
					}
				}
			}

			Tile tileToAdd;

			tiles.TryGetValue(matchingTile, out tileToAdd);

			if (tileToAdd != null)
			{
				Rect tileRect = tileToAdd.Rects[0];
				float xPositionRect = (i == 0 || i == 3) ? 0 : tileRect.width / 2;
				float yPositionRect = i > 1 ? 0 : tileRect.height / 2;
				Rect newRect = new Rect(xPositionRect, yPositionRect, tileRect.width / 2, tileRect.height / 2);

				Rect tileUvRect = tileToAdd.UvRects[0];
				float xPositionUvRect = (i == 0 || i == 3) ? tileUvRect.x : tileUvRect.x + tileUvRect.width / 2;
				float yPositionUvRect = i > 1 ? tileUvRect.y : tileUvRect.y + tileUvRect.height / 2;
				Rect newUvRect = new Rect(xPositionUvRect, yPositionUvRect, tileUvRect.width / 2, tileUvRect.height / 2);

				rects.Add(newRect);
				uvRects.Add(newUvRect);
			}

			currentMask = currentMask.RotateRight(2);
		}

		// TODO: merge adjacent equal tiles

		result = new Tile(0, rects.ToArray(), uvRects.ToArray());

		return result;
	}

	private bool MaskedBytesMatch(byte mask, byte a, byte b)
	{
		return (a & mask) == (b & mask);
	}

	private byte FindClosestTile(byte mask, Dictionary<byte, Tile> tileSet)
	{
		byte result = 0;
		int minValue = int.MaxValue;
		int distance;

		foreach (var tile in tileSet)
		{
			distance = HammingDistance(mask, tile.Key);

			if (distance < minValue)
			{
				result = tile.Key;
				minValue = distance;
			}
		}

		return result;
	}

	private int HammingDistance(byte a, byte b)
	{
		int result = 0;

		byte mask = (byte)(a ^ b);

		for (int i = 0; i < 8; i++)
		{
			result += (mask >> i) & 1;
		}

		return result;
	}
}
