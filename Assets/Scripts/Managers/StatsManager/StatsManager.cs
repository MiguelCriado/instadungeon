using InstaDungeon.Events;
using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

namespace InstaDungeon
{
	public class StatsManager : Manager
	{
		private readonly static string DefaultSavePath = Path.Combine(Application.streamingAssetsPath, "Sessions Log");

		private StatsSessionInfo sessionInfo;
		private MapStats currentMapStats;
		private int stepsPerformed;

		public StatsManager() : base(true, false)
		{
			sessionInfo = new StatsSessionInfo();
			SubscribeListeners();
		}

		private void StartSession()
		{
			sessionInfo.StartSession();

			DirectoryInfo saveDirectory = new DirectoryInfo(DefaultSavePath);

			if (saveDirectory.Exists == false)
			{
				Directory.CreateDirectory(saveDirectory.FullName);
			}
		}

		private void EndSession()
		{
			sessionInfo.EndSession();
			SaveToFile();
		}

		private void SaveToFile()
		{
			string layoutGenerator = sessionInfo.StatList[0].LayoutGenerator;
			string zoneGenerator = sessionInfo.StatList[0].ZoneGenerator;
			int initialSeed = sessionInfo.StatList[0].Seed;
			string dateTime = DateTime.UtcNow.ToString("s").Replace(":", ".");
			string fileName = string.Format("{0} - {1}, {2} ({3}).json", dateTime, layoutGenerator, zoneGenerator, initialSeed);
			string filePath = Path.Combine(DefaultSavePath, fileName);

			using (FileStream fs = File.Open(filePath, FileMode.CreateNew))
			using (StreamWriter sw = new StreamWriter(fs))
			using (JsonWriter jw = new JsonTextWriter(sw))
			{
				jw.Formatting = Formatting.Indented;
				JsonSerializer serializer = new JsonSerializer();
				serializer.Serialize(jw, sessionInfo);
			}
		}

		#region [Event Reactions]

		private void OnGameStarts(IEventData eventData)
		{
			StartSession();
		}

		private void OnMapGenerated(IEventData eventData)
		{
			MapGenerationNewMapEvent mapEvent = eventData as MapGenerationNewMapEvent;

			currentMapStats = new MapStats();
			currentMapStats.Level = mapEvent.Level;
			currentMapStats.Seed = mapEvent.LevelSeed;
			currentMapStats.LayoutGenerator = mapEvent.Generator.LayoutGenerator.ToString();
			currentMapStats.ZoneGenerator = mapEvent.Generator.ZoneGenerator.ToString();
			currentMapStats.Topology = AnalyzeMapTopology(mapEvent.Map);

			stepsPerformed = 0;
		}

		private void OnLevelFinished(IEventData eventData)
		{
			LevelFinishedEvent levelEvent = eventData as LevelFinishedEvent;

			currentMapStats.Navigation = AnalyzeMapNavigation(levelEvent.Map);
			sessionInfo.AddStats(currentMapStats);
		}

		private void OnPlayerStep(IEventData eventData)
		{
			EntityFinishMovementEvent entityEvent = eventData as EntityFinishMovementEvent;

			if (entityEvent.EntityId == Locator.Get<GameManager>().Player.Guid)
			{
				stepsPerformed++;
			}
		}

		private void OnGameOver(IEventData eventData)
		{
			GameStateChangeEvent stateEvent = eventData as GameStateChangeEvent;

			if (stateEvent.State == GameState.GameOver)
			{
				EndSession();
			}
		}

		#endregion

		#region [Helpers]

		private void SubscribeListeners()
		{
			GameManager gameManager = Locator.Get<GameManager>();
			gameManager.Events.AddListener(OnGameStarts, NewGameStartsEvent.EVENT_TYPE);
			gameManager.Events.AddListener(OnLevelFinished, LevelFinishedEvent.EVENT_TYPE);
			gameManager.Events.AddListener(OnGameOver, GameStateChangeEvent.EVENT_TYPE);

			Locator.Get<MapGenerationManager>().Events.AddListener(OnMapGenerated, MapGenerationNewMapEvent.EVENT_TYPE);
			Locator.Get<EntityManager>().Events.AddListener(OnPlayerStep, EntityFinishMovementEvent.EVENT_TYPE);
		}

		private MapTopologyStats AnalyzeMapTopology(TileMap<Cell> map)
		{
			MapTopologyStats result = new MapTopologyStats();

			result.MapBounds = map.Bounds;
			result.FillRate = CalculateFillRate(map);
			result.NumZones = map.Layout.Zones.Count;
			result.BiggestZone = GetBiggestZone(map);
			result.SmallestZone = GetSmallestZone(map);
			result.BiggestZoneRatio = (float)(result.SmallestZone.width * result.SmallestZone.height) / (result.BiggestZone.width * result.BiggestZone.height);
			result.MeanZoneConnectionsNumber = GetMeanZoneConnections(map);
			result.DeadEndedZones = GetDeadEndedZones(map);

			return result;
		}

		private static float CalculateFillRate(TileMap<Cell> map)
		{
			var enumerator = map.GetEnumerator();
			int floorTiles = 0;

			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Value.TileInfo.TileType == TileType.Floor)
				{
					floorTiles++;
				}
			}

			int totalTiles = map.Bounds.width * map.Bounds.height;
			return (float)floorTiles / totalTiles;
		}

		private static RectangleInt GetBiggestZone(TileMap<Cell> map)
		{
			RectangleInt result = new RectangleInt(0, 0, 0, 0);
			var enumerator = map.Layout.Zones.Nodes.GetEnumerator();
			int biggestArea = 0;

			while (enumerator.MoveNext())
			{
				RectangleInt bounds = enumerator.Current.Value.bounds;

				if (bounds.width * bounds.height > biggestArea)
				{
					result = bounds;
				}
			}

			return result;
		}

		private static RectangleInt GetSmallestZone(TileMap<Cell> map)
		{
			RectangleInt result = new RectangleInt(0, 0, 0, 0);
			var enumerator = map.Layout.Zones.Nodes.GetEnumerator();
			int smallestArea = int.MaxValue;

			while (enumerator.MoveNext())
			{
				RectangleInt bounds = enumerator.Current.Value.bounds;

				if (bounds.width * bounds.height < smallestArea)
				{
					result = bounds;
				}
			}

			return result;
		}

		private static float GetMeanZoneConnections(TileMap<Cell> map)
		{
			float result = 0;
			var enumerator = map.Layout.Zones.Nodes.GetEnumerator();

			while (enumerator.MoveNext())
			{
				GraphNode<Zone> zone = enumerator.Current as GraphNode<Zone>;
				result += zone.Neighbors.Count;
			}

			result = result / map.Layout.Zones.Count;
			return result;
		}

		private static int GetDeadEndedZones(TileMap<Cell> map)
		{
			int result = 0;
			var enumerator = map.Layout.Zones.Nodes.GetEnumerator();

			while (enumerator.MoveNext())
			{
				GraphNode<Zone> zone = enumerator.Current as GraphNode<Zone>;

				if (zone.Neighbors.Count <= 1 && zone.Value != map.Layout.InitialZone && zone.Value != map.Layout.FinalZone)
				{
					result++;
				}
			}

			return result;
		}

		private MapNavigationStats AnalyzeMapNavigation(TileMap<Cell> map)
		{
			MapNavigationStats result = new MapNavigationStats();
			result.StepsInDirectPath = GetStepsInDirectPath(map);
			result.ExploredMap = GetExploredMapPercentage(map);
			result.StepsPerformed = stepsPerformed;
			result.VisitedZonesPercentage = GetVisitedZonesPercentage(map);
			return result;
		}

		private static int GetStepsInDirectPath(TileMap<Cell> map)
		{
			int result = 0;
			int2[] path = Locator.Get<MapManager>().GetPathIgnoringActors(map.SpawnPoint, map.ExitPoint);

			if (path != null)
			{
				result = path.Length;
			}

			return result;
		}

		private static float GetExploredMapPercentage(TileMap<Cell> map)
		{
			int totalFloorTiles = 0;
			int exploredFloorTiles = 0;
			var enumerator = map.GetEnumerator();

			while (enumerator.MoveNext())
			{
				Cell cell = enumerator.Current.Value;

				if (cell.TileInfo.TileType == TileType.Floor)
				{
					totalFloorTiles++;

					if (cell.Visibility == VisibilityType.Visible || cell.Visibility == VisibilityType.PreviouslySeen)
					{
						exploredFloorTiles++;
					}
				}
			}

			return ((float)exploredFloorTiles / totalFloorTiles) * 100;
		}

		private static float GetVisitedZonesPercentage(TileMap<Cell> map)
		{
			int zonesVisited = 0;
			var enumerator = map.Layout.Zones.Nodes.GetEnumerator();

			while (enumerator.MoveNext())
			{
				Zone zone = enumerator.Current.Value;
				bool found = false;
				var tilesEnumerator = zone.GetEnumerator();

				while (found == false && tilesEnumerator.MoveNext())
				{
					Cell cell = map[tilesEnumerator.Current];

					if (cell.TileInfo.TileType == TileType.Floor && cell.Visibility != VisibilityType.Obscured)
					{
						zonesVisited++;
						found = true;
					}
				}
			}

			return (float)zonesVisited / map.Layout.Zones.Count;
		}

		#endregion
	}
}
