﻿using AI.BehaviorTrees;
using InstaDungeon.Components;
using UnityEngine;
using System.Collections.Generic;

namespace InstaDungeon.BehaviorTreeNodes
{
	public class PickClosestRandomTileInThresholdAction : ActionNode
	{
		private string tileId;
		private string thresholdId;
		private ManhattanDistanceHeuristic heuristic;

		public PickClosestRandomTileInThresholdAction(string tileIdInBlackboard, string thresholdIdInBlackboard)
		{
			tileId = tileIdInBlackboard;
			thresholdId = thresholdIdInBlackboard;
			heuristic = new ManhattanDistanceHeuristic();
		}

		protected override NodeStates Tick(Tick tick)
		{
			NodeStates result = NodeStates.Error;

			HashSet<int2> threshold = null;

			if (tick.Blackboard.TryGet(thresholdId, out threshold))
			{
				Entity target = tick.Target as Entity;
				int2 targetPosition = target.CellTransform.Position;
				MapManager mapManager = Locator.Get<MapManager>();
				List<int2> candidates = new List<int2>();
				int closestDistance = int.MaxValue;
				var enumerator = threshold.GetEnumerator();
				
				while (enumerator.MoveNext())
				{
					int distance = heuristic.Evaluate(targetPosition, enumerator.Current);

					if (distance < closestDistance)
					{
						closestDistance = distance;
						candidates.Clear();
						candidates.Add(enumerator.Current);
					}
					else if (distance == closestDistance)
					{
						candidates.Add(enumerator.Current);
					}
				}

				if (candidates.Count > 0)
				{
					int2 chosenDestiny = candidates[Random.Range(0, candidates.Count)];
					int2 currentDestiny;

					if (tick.Blackboard.TryGet(tileId, out currentDestiny))
					{
						int2[] path = mapManager.GetPathIgnoringActors(targetPosition, currentDestiny);
						Cell cell = mapManager[currentDestiny.x, currentDestiny.y];

						if (threshold.Contains(currentDestiny) && path.Length > 0 && path.Length <= closestDistance)
						{
							chosenDestiny = currentDestiny;
						}
					}

					tick.Blackboard.Set(tileId, chosenDestiny);
					result = NodeStates.Success;
				}
				else
				{
					tick.Blackboard.Remove(tileId);
					result = NodeStates.Failure;
				}
			}

			return result;
		}
	}
}
