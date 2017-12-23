using InstaDungeon.MapGeneration;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using MoonSharp.VsCodeDebugger;

namespace InstaDungeon
{
	public class ScriptingManager : Manager
	{
		private static readonly string ScriptsPath = Path.Combine(Application.streamingAssetsPath, "Generators");
		private static readonly string LayoutGeneratorsPath = Path.Combine(ScriptsPath, "Layout");
		private static readonly string ZoneGeneratorsPath = Path.Combine(ScriptsPath, "Zone");

		private Dictionary<string, ScriptLayoutGenerator> layoutGenerators;
		private Dictionary<string, ScriptZoneGenerator> zoneGenerators;
		private bool debug;
		private MoonSharpVsCodeDebugServer server;

		public ScriptingManager() : base(true, false)
		{
			layoutGenerators = new Dictionary<string, ScriptLayoutGenerator>();
			zoneGenerators = new Dictionary<string, ScriptZoneGenerator>();
			SetupDebugEnvironment();
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

		public ScriptLayoutGenerator GetLayoutGenerator(string name)
		{
			ScriptLayoutGenerator result = null;
			layoutGenerators.TryGetValue(name, out result);
			return result;
		}

		public ScriptZoneGenerator GetZoneGenerator(string name)
		{
			ScriptZoneGenerator result = null;
			zoneGenerators.TryGetValue(name, out result);
			return result;
		}

		public void SetRandomSeed(int seed)
		{
			var layoutEnumerator = layoutGenerators.Values.GetEnumerator();

			while (layoutEnumerator.MoveNext())
			{
				layoutEnumerator.Current.SetRandomSeed(seed);
			}

			var zoneEnumerator = zoneGenerators.Values.GetEnumerator();

			while (zoneEnumerator.MoveNext())
			{
				zoneEnumerator.Current.SetRandomSeed(seed);
			}
		}

		private void SetupDebugEnvironment() 
		{
			debug = true;

			if (debug) 
			{
				server = new MoonSharpVsCodeDebugServer();
				server.Start();
			}
		}

		private void LoadLayoutGenerationScripts()
		{
			DirectoryInfo dirInfo = new DirectoryInfo(LayoutGeneratorsPath);
			DirectoryInfo[] generatorDirectories = dirInfo.GetDirectories();

			for (int i = 0; i < generatorDirectories.Length; i++)
			{
				FileInfo generatorFile = new FileInfo(Path.Combine(generatorDirectories[i].FullName, "generator.lua"));
				FileInfo settingsFile = new FileInfo(Path.Combine(generatorDirectories[i].FullName, "settings.json"));

				if (generatorFile.Exists && settingsFile.Exists)
				{
					try
					{
						string generatorName = generatorDirectories[i].Name;
						Script script = LoadScript(generatorFile.FullName);
						string settingsString = File.ReadAllText(settingsFile.FullName);
						ScriptLayoutGenerator generator = new ScriptLayoutGenerator(generatorName, script, settingsString);
						layoutGenerators.Add(generatorName, generator);

						if (debug)
						{
							server.AttachToScript(script, generatorName);
						}
					}
					catch (SyntaxErrorException e)
					{
						Debug.Log(e.DecoratedMessage);
						e.Rethrow();
					}
				}
			}
		}

		private void LoadZoneGenerationScripts()
		{
			DirectoryInfo dirInfo = new DirectoryInfo(ZoneGeneratorsPath);
			DirectoryInfo[] generatorDirectories = dirInfo.GetDirectories();

			for (int i = 0; i < generatorDirectories.Length; i++)
			{
				FileInfo generatorFile = new FileInfo(Path.Combine(generatorDirectories[i].FullName, "generator.lua"));
				FileInfo settingsFile = new FileInfo(Path.Combine(generatorDirectories[i].FullName, "settings.json"));

				if (generatorFile.Exists && settingsFile.Exists)
				{
					try
					{
						string generatorName = generatorDirectories[i].Name;
						Script script = LoadScript(generatorFile.FullName);
						string settingsString = File.ReadAllText(settingsFile.FullName);
						ScriptZoneGenerator generator = new ScriptZoneGenerator(generatorName, script, settingsString);
						zoneGenerators.Add(generatorName, generator);

						if (debug)
						{
							server.AttachToScript(script, generatorName);
						}
					}
					catch (SyntaxErrorException e)
					{
						Debug.Log(e.DecoratedMessage);
						e.Rethrow();
					}
				}
			}
		}

		private static Script LoadScript(string filePath)
		{
			Script script = new Script();
			((ScriptLoaderBase)script.Options.ScriptLoader).ModulePaths = new string[] { "?_module" };
			string scriptString = File.ReadAllText(filePath);
			string randomSeedFunction = @"
				function set_random_seed(seed)
					math.randomseed(seed)
				end";
			scriptString += randomSeedFunction;
			script.DoString(scriptString);
			return script;
		}

		#region [Custom Converters]

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
				return GetZone(script, value, null);
			});

			Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.Table, typeof(Zone), (value) => 
			{
				return GetZone(value);
			});

			// Layout

			Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion<Layout>((script, value) =>
			{
				return GetLayout(script, value, null);
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

		private static DynValue GetZone(Script script, Zone zone, TileMap<TileType> map)
		{
			Table table = new Table(script);
			table[DynValue.NewString("id")] = DynValue.NewNumber(zone.id);
			table[DynValue.NewString("min_bound")] = GetInt2(script, zone.bounds.Min.x, zone.bounds.Min.y);
			table[DynValue.NewString("max_bound")] = GetInt2(script, zone.bounds.Max.x, zone.bounds.Max.y);
			table[DynValue.NewString("connections")] = GetZoneConnections(script, zone.connections);
			table[DynValue.NewString("tiles")] = GetZoneTiles(script, zone.tiles, map, zone.bounds.Min, zone.bounds.Max);
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

		private static DynValue GetLayout(Script script, Layout layout, TileMap<TileType> map)
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
				zonesTable[DynValue.NewNumber(currentZone.id)] = GetZone(script, currentZone, map);

				Table currentZoneConectionsTable = new Table(script);
				connectionsTable[DynValue.NewNumber(currentZone.id)] = DynValue.NewTable(currentZoneConectionsTable);
				var connectionsEnumerator = layout.GetAdjacentZones(currentZone).GetEnumerator();

				while (connectionsEnumerator.MoveNext())
				{
					Zone currentConnectionZone = connectionsEnumerator.Current.Value;
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

					if (result.GetAdjacentZones(currentZone).FindByValue(newConnectedZone) == null)
					{
						result.ConnectZones(currentZone, newConnectedZone);
					}
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
			table[DynValue.NewString("layout")] = GetLayout(script, map.Layout, map);
			table[DynValue.NewString("min_bound")] = GetInt2(script, map.Bounds.Min.x, map.Bounds.Min.y);
			table[DynValue.NewString("max_bound")] = GetInt2(script, map.Bounds.Max.x, map.Bounds.Max.y);
			table[DynValue.NewString("spawn_point")] = GetInt2(script, map.SpawnPoint.x, map.SpawnPoint.y);
			table[DynValue.NewString("exit_point")] = GetInt2(script, map.ExitPoint.x, map.ExitPoint.y);
			return DynValue.NewTable(table);
		}

		private static TileMap<TileType> GetTileMap(DynValue mapValue)
		{
			TileMap<TileType> result = new TileMap<TileType>();
			Table table = mapValue.Table;
			result.Layout = GetLayout(table.Get(DynValue.NewString("layout")));
			result.SpawnPoint = GetInt2(table.Get(DynValue.NewString("spawn_point")));
			result.ExitPoint = GetInt2(table.Get(DynValue.NewString("exit_point")));
			AddFloorTiles(result);
			WrapFloorWithWallTiles(result);

			return result;
		}

		// #### TileMap Helpers

		private static void AddFloorTiles(TileMap<TileType> result)
		{
			var zoneEnumerator = result.Layout.Zones.Nodes.GetEnumerator();

			while (zoneEnumerator.MoveNext())
			{
				Zone zone = zoneEnumerator.Current.Value;

				var tileEnumerator = zone.GetEnumerator();

				while (tileEnumerator.MoveNext())
				{
					result.Add(tileEnumerator.Current, TileType.Floor);
				}
			}
		}

		private static void WrapFloorWithWallTiles(TileMap<TileType> result)
		{
			int2[] directions = new int2[]
			{
				new int2(0, 1),
				new int2(1, 1),
				new int2(1, 0),
				new int2(1, -1),
				new int2(0, -1),
				new int2(-1, -1),
				new int2(-1, 0),
				new int2(-1, 1)
			};

			int2[] floorTiles = result.GetPresentTiles();

			for (int i = 0; i < floorTiles.Length; i++)
			{
				for (int j = 0; j < directions.Length; j++)
				{
					int2 targetTile = floorTiles[i] + directions[j];

					if (result[targetTile] != TileType.Floor)
					{
						Zone zone = result.Layout.FindZoneByPosition(targetTile);

						if (zone != null)
						{
							zone.tiles.Add(targetTile);
							result.Add(targetTile, TileType.Wall);
						}
					}
				}
			}
		}

		// #### Zone Helpers

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

		private static DynValue GetZoneTiles(Script script, HashSet<int2> tiles, TileMap<TileType> map, int2 minBounds, int2 maxBounds)
		{
			Table result = new Table(script);

			for (int x = minBounds.x; x < maxBounds.x; x++)
			{
				DynValue xIndex = DynValue.NewNumber(x);
				result[xIndex] = DynValue.NewTable(script);

				for (int y = minBounds.y; y < maxBounds.y; y++)
				{
					DynValue yIndex = DynValue.NewNumber(y);
					int2 position = new int2(x, y);

					if (tiles.Contains(position) && (map == null || map[position] == TileType.Floor))
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

				if (tilesTable.Get(xIndex) != DynValue.Nil)
				{
					for (int y = minBound.y; y < maxBound.y; y++)
					{
						DynValue yIndex = DynValue.NewNumber(y);
						DynValue value = tilesTable.Get(xIndex, yIndex);

						if (value != DynValue.Nil && value.Boolean == true)
						{
							zone.tiles.Add(new int2(x, y));
						}
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
	}

	#endregion
}
