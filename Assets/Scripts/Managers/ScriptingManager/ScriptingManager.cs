using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace InstaDungeon
{
	public class ScriptingManager : Manager
	{
		private static readonly string ScriptsPath = Path.Combine(Application.streamingAssetsPath, "Generators");
		private static readonly string LayoutGeneratorsPath = Path.Combine(ScriptsPath, "Layout");
		private static readonly string ZoneGeneratorsPath = Path.Combine(ScriptsPath, "Zone");

		private Dictionary<string, Script> layoutGenerators;
		private Dictionary<string, Script> zoneGenerators;

		public ScriptingManager() : base(true, false)
		{
			layoutGenerators = new Dictionary<string, Script>();
			zoneGenerators = new Dictionary<string, Script>();

			// Script.DefaultOptions.ScriptLoader = new ReplInterpreterScriptLoader();

			LoadLayoutGenerationScripts();
			LoadZoneGenerationScripts();
			RegisterCustomConverters();
		}

		public List<string> GetLayoutGeneratorNames()
		{
			return new List<string>(layoutGenerators.Keys);
		}

		public List<string> GetZoneGeneratorNames()
		{
			return new List<string>(zoneGenerators.Keys);
		}

		public Script GetLayoutGenerator(string name)
		{
			Script result = null;
			layoutGenerators.TryGetValue(name, out result);
			return result;
		}

		public Script GetZoneGenerator(string name)
		{
			Script result = null;
			zoneGenerators.TryGetValue(name, out result);
			return result;
		}

		private void LoadLayoutGenerationScripts()
		{
			LoadScriptsIntoDictionary(LayoutGeneratorsPath, layoutGenerators);
		}

		private void LoadZoneGenerationScripts()
		{
			LoadScriptsIntoDictionary(ZoneGeneratorsPath, zoneGenerators);
		}

		private void LoadScriptsIntoDictionary(string path, Dictionary<string, Script> dictionary)
		{
			DirectoryInfo dirInfo = new DirectoryInfo(path);
			FileInfo[] files = dirInfo.GetFiles("*.lua");

			for (int i = 0; i < files.Length; i++)
			{
				Script script = new Script();
				((ScriptLoaderBase)script.Options.ScriptLoader).ModulePaths = new string[] { "?_module" };
				string scriptString = File.ReadAllText(files[i].FullName);
				script.DoString(scriptString);
				dictionary.Add(files[i].Name, script);
			}
		}

		private void RegisterCustomConverters()
		{
			// int2

			Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion<int2>((script, value) => 
			{
				return GetInt2(script, value.x, value.y);
			});

			Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.Table, typeof(int2), (value) => 
			{
				return GetInt2(value);
			});

			// Zone

			Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion<Zone>((script, value) => 
			{
				return GetZone(script, value);
			});

			Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.Table, typeof(Zone), (value) => 
			{
				return GetZone(value);
			});

			// Layout

			Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion<Layout>((script, value) =>
			{
				return GetLayout(script, value);
			});

			Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.Table, typeof(Layout), (value) =>
			{
				return GetLayout(value);
			});

			// TileMap<TileType>

			Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion<TileMap<TileType>>((script, value) => 
			{
				return GetTileMap(script, value);
			});

			Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.Table, typeof(TileMap<TileType>), (value) => 
			{
				return GetTileMap(value);
			});
		}

		private static DynValue GetInt2(Script script, int x, int y)
		{
			Table table = new Table(script);
			table[DynValue.NewString("x")] = DynValue.NewNumber(x);
			table[DynValue.NewString("y")] = DynValue.NewNumber(y);
			return DynValue.NewTable(table);
		}

		private static int2 GetInt2(DynValue int2Value)
		{
			int x = (int)int2Value.Table.Get(DynValue.NewString("x")).Number;
			int y = (int)int2Value.Table.Get(DynValue.NewString("y")).Number;
			return new int2(x, y);
		}

		private static DynValue GetZone(Script script, Zone zone)
		{
			Table table = new Table(script);
			table[DynValue.NewString("id")] = DynValue.NewNumber(zone.id);
			table[DynValue.NewString("min_bound")] = GetInt2(script, zone.bounds.Min.x, zone.bounds.Min.y);
			table[DynValue.NewString("max_bound")] = GetInt2(script, zone.bounds.Max.x, zone.bounds.Max.y);
			table[DynValue.NewString("connections")] = GetZoneConnections(script, zone.connections);
			table[DynValue.NewString("tiles")] = GetZoneTiles(script, zone.tiles, zone.bounds.Min, zone.bounds.Max);
			return DynValue.NewTable(table);
		}

		private static Zone GetZone(DynValue zoneValue)
		{
			Table table = zoneValue.Table;

			int id = (int)table.Get(DynValue.NewString("id")).Number;
			int2 minBound = table.Get(DynValue.NewString("min_bound")).ToObject<int2>();
			int2 maxBound = table.Get(DynValue.NewString("max_bound")).ToObject<int2>();
			Zone result = new Zone(id, minBound.x, minBound.y, maxBound.x - minBound.x, maxBound.y - minBound.y);

			AddZoneTiles(table, minBound, maxBound, result);
			AddZoneConnections(table, result);

			return result;
		}

		private static DynValue GetLayout(Script script, Layout layout)
		{
			Table result = new Table(script);

			result[DynValue.NewString("initial_zone")] = DynValue.NewNumber(GetZoneId(layout.InitialZone));
			result[DynValue.NewString("final_zone")] = DynValue.NewNumber(GetZoneId(layout.FinalZone));

			Table zonesTable = new Table(script);
			result[DynValue.NewString("zones")] = DynValue.NewTable(zonesTable);
			Table connectionsTable = new Table(script);
			result[DynValue.NewString("connections")] = DynValue.NewTable(connectionsTable);
			var enumerator = layout.Zones.Nodes.GetEnumerator();

			while (enumerator.MoveNext())
			{
				Zone currentZone = enumerator.Current.Value;
				zonesTable[DynValue.NewNumber(currentZone.id)] = GetZone(script, currentZone);

				Table currentZoneConectionsTable = new Table(script);
				connectionsTable[DynValue.NewNumber(currentZone.id)] = DynValue.NewTable(currentZoneConectionsTable);
				var connectionsEnumerator = layout.GetAdjacentZones(currentZone).GetEnumerator();

				while (connectionsEnumerator.MoveNext())
				{
					Zone currentConnectionZone = enumerator.Current.Value;
					currentZoneConectionsTable.Append(DynValue.NewNumber(currentConnectionZone.id));
				}
			}

			return DynValue.NewTable(result);
		}

		private static Layout GetLayout(DynValue layoutValue)
		{
			Layout result = new Layout();

			Table table = layoutValue.Table;
			Table zonesTable = table.Get(DynValue.NewString("zones")).Table;

			foreach (var zone in zonesTable.Values)
			{
				Zone newZone = GetZone(zone);
				result.Add(newZone);
			}

			Table connectionsTable = table.Get(DynValue.NewString("connections")).Table;

			foreach (var pair in connectionsTable.Pairs)
			{
				int zoneId = (int)pair.Key.Number;
				Zone currentZone = result.Zones.Find(x => x.id == zoneId);
				Table zoneConnectionsTable = pair.Value.Table;

				foreach (var connectedZone in zoneConnectionsTable.Values)
				{
					Zone newConnectedZone = result.Zones.Find(x => x.id == (int)connectedZone.Number);
					result.ConnectZones(currentZone, newConnectedZone);
				}
			}

			int initialZoneId = (int)table.Get(DynValue.NewString("initial_zone")).Number;
			result.InitialZone = result.Zones.Find(x => x.id == initialZoneId);
			int finalZoneId = (int)table.Get(DynValue.NewString("final_zone")).Number;
			result.FinalZone = result.Zones.Find(x => x.id == finalZoneId);

			return result;
		}

		private static DynValue GetTileMap(Script script, TileMap<TileType> map)
		{
			Table table = new Table(script);
			table[DynValue.NewString("layout")] = GetLayout(script, map.Layout);
			table[DynValue.NewString("min_bound")] = GetInt2(script, map.Bounds.Min.x, map.Bounds.Min.y);
			table[DynValue.NewString("max_bound")] = GetInt2(script, map.Bounds.Max.x, map.Bounds.Max.y);
			table[DynValue.NewString("spawn_point")] = GetInt2(script, map.SpawnPoint.x, map.SpawnPoint.y);
			table[DynValue.NewString("exit_point")] = GetInt2(script, map.ExitPoint.x, map.ExitPoint.y);
			table[DynValue.NewString("tiles")] = GetMapTiles(script, map);
			return DynValue.NewTable(table);
		}

		private static TileMap<TileType> GetTileMap(DynValue mapValue)
		{
			TileMap<TileType> result = new TileMap<TileType>();
			Table table = mapValue.Table;
			result.Layout = GetLayout(table.Get(DynValue.NewString("layout")));
			result.SpawnPoint = GetInt2(table.Get(DynValue.NewString("spawn_point")));
			result.ExitPoint = GetInt2(table.Get(DynValue.NewString("exit_point")));
			AddMapTiles(table, result);
			return result;
		}

		private static DynValue GetZoneConnections(Script script, Dictionary<int2, Zone> connections)
		{
			Table table = new Table(script);
			var enumerator = connections.GetEnumerator();

			while (enumerator.MoveNext())
			{
				var connection = enumerator.Current;
				DynValue zoneId = DynValue.NewNumber(connection.Value.id);

				if (table.Get(zoneId) == DynValue.Nil)
				{
					table[zoneId] = DynValue.NewTable(script);
				}

				table.Get(zoneId).Table.Append(GetInt2(script, connection.Key.x, connection.Key.y));
			}

			return DynValue.NewTable(table);
		}

		private static DynValue GetZoneTiles(Script script, HashSet<int2> tiles, int2 minBounds, int2 maxBounds)
		{
			Table result = new Table(script);

			for (int x = minBounds.x; x < maxBounds.x; x++)
			{
				DynValue xIndex = DynValue.NewNumber(x);
				result[xIndex] = DynValue.NewTable(script);

				for (int y = minBounds.y; y < maxBounds.y; y++)
				{
					DynValue yIndex = DynValue.NewNumber(y);

					if (tiles.Contains(new int2(x, y)))
					{
						result[xIndex, yIndex] = DynValue.NewBoolean(true);
					}
					else
					{
						result[xIndex, yIndex] = DynValue.NewBoolean(false);
					}
				}
			}

			return DynValue.NewTable(result);
		}

		private static void AddZoneTiles(Table zoneTable, int2 minBound, int2 maxBound, Zone zone)
		{
			Table tilesTable = zoneTable.Get(DynValue.NewString("tiles")).Table;

			for (int x = minBound.x; x < maxBound.x; x++)
			{
				DynValue xIndex = DynValue.NewNumber(x);

				for (int y = minBound.y; y < maxBound.y; y++)
				{
					DynValue yIndex = DynValue.NewNumber(y);
					bool value = tilesTable.Get(xIndex, yIndex).Boolean;

					if (value == true)
					{
						zone.tiles.Add(new int2(x, y));
					}
				}
			}
		}

		private static void AddZoneConnections(Table zoneTable, Zone zone)
		{
			DynValue connections = zoneTable.Get(DynValue.NewString("connections"));
			Table connectionsTable = connections.Table;

			foreach (var pair in connectionsTable.Pairs)
			{
				int zoneId = (int)pair.Key.Number;

				Table pointsTable = pair.Value.Table;

				foreach (var point in pointsTable.Values)
				{
					zone.AddConnectionPoint(point.ToObject<int2>(), new Zone(zoneId));
				}
			}
		}

		private static int GetZoneId(Zone zone)
		{
			int result = -1;

			if (zone != null)
			{
				result = zone.id;
			}

			return result;
		}

		private static DynValue GetMapTiles(Script script, TileMap<TileType> map)
		{
			Table table = new Table(script);

			for (int x = map.Bounds.Min.x; x < map.Bounds.Max.x; x++)
			{
				DynValue xIndex = DynValue.NewNumber(x);
				table[xIndex] = DynValue.NewTable(script);

				for (int y = map.Bounds.Min.y; y < map.Bounds.Max.y; y++)
				{
					DynValue yIndex = DynValue.NewNumber(y);
					TileType tile = map.GetTile(x, y);
					table[xIndex, yIndex] = DynValue.NewNumber((int)tile);
				}
			}

			return DynValue.NewTable(table);
		}

		private static void AddMapTiles(Table mapTable, TileMap<TileType> map)
		{
			int2 minBound = GetInt2(mapTable.Get(DynValue.NewString("min_bound")));
			int2 maxBound = GetInt2(mapTable.Get(DynValue.NewString("max_bound")));
			Table tilesTable = mapTable.Get(DynValue.NewString("tiles")).Table;

			for (int x = minBound.x; x < maxBound.x; x++)
			{
				DynValue xIndex = DynValue.NewNumber(x);

				for (int y = minBound.y; y < maxBound.y; y++)
				{
					DynValue yIndex = DynValue.NewNumber(y);
					TileType tileType = (TileType)(int)tilesTable.Get(xIndex, yIndex).Number;

					if (tileType != TileType.Space)
					{
						map.Add(x, y, tileType);
					}
				}
			}
		}
	}
}
