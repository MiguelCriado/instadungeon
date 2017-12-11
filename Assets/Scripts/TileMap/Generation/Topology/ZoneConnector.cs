using MoonSharp.Interpreter;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InstaDungeon.MapGeneration
{
	public static class ZoneConnector
	{
		public static TileMap<TileType> BuildMap(ILayoutGenerator layoutGenerator, IZoneGenerator zoneGenerator, int level, int levelSeed)
		{
			Random.InitState(levelSeed);
			TileMap<TileType> result = new TileMap<TileType>();

			try
			{
				result.Layout = layoutGenerator.NewLayout(level);

				while (!layoutGenerator.IsDone(result.Layout, level))
				{
					result.Layout = layoutGenerator.Iterate(result.Layout, level);

					result = zoneGenerator.PreConnectZones(result, level);
					result = zoneGenerator.Generate(result, level);
					result = zoneGenerator.PostConnectZones(result, level);
				}

				result.SpawnPoint = zoneGenerator.GetSpawnPoint(result, level);
				result.ExitPoint = zoneGenerator.GetExitPoint(result, level);
			}
			catch (ScriptRuntimeException e)
			{
				Debug.Log(e.DecoratedMessage);
			}

			return result;
		}
	}
}
