using InstaDungeon.Components;
using InstaDungeon.Events;
using System;
using System.Collections.Generic;

namespace InstaDungeon
{
	public enum ConflictSide
	{
		Heroes,
		Monsters
	}

	public class SideManager : Manager
	{
		private MapManager mapManager;

		private Dictionary<ConflictSide, List<Entity>> actors;

		public SideManager() : base(false, false)
		{
			mapManager = Locator.Get<MapManager>();

			InitializeActorsDictionary();
			IncludePresentActors();
			SuscribeListeners();
		}

		public List<Entity> GetActors(ConflictSide side)
		{
			return actors[side];
		}

		public List<Entity> GetOtherSidesActors(ConflictSide side)
		{
			List<Entity> result = new List<Entity>();
			var enumerator = actors.GetEnumerator();

			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Key != side)
				{
					result.AddRange(enumerator.Current.Value);
				}
			}

			return result;
		}

		#region [Event Reactions]

		private void OnActorAddedToMap(IEventData eventData)
		{
			ActorAddedToMapEvent mapEvent = eventData as ActorAddedToMapEvent;
			AddSideActor(mapEvent.Actor);
		}

		private void OnActorRemovedFromMap(IEventData eventData)
		{
			ActorRemovedFromMapEvent mapEvent = eventData as ActorRemovedFromMapEvent;
			RemoveSideActor(mapEvent.Actor);
		}

		#endregion

		#region [Helpers]

		private void InitializeActorsDictionary()
		{
			actors = new Dictionary<ConflictSide, List<Entity>>();
			var sideList = Enum.GetValues(typeof(ConflictSide)).GetEnumerator();

			while (sideList.MoveNext())
			{
				actors.Add((ConflictSide)sideList.Current, new List<Entity>());
			}
		}

		private void SuscribeListeners()
		{
			mapManager.Events.AddListener(OnActorAddedToMap, ActorAddedToMapEvent.EVENT_TYPE);
			mapManager.Events.AddListener(OnActorRemovedFromMap, ActorRemovedFromMapEvent.EVENT_TYPE);
		}

		private void IncludePresentActors()
		{
			List<Entity> actorsInMap = mapManager.GetActors();

			for (int i = 0; i < actorsInMap.Count; i++)
			{
				AddSideActor(actorsInMap[i]);
			}
		}

		private void AddSideActor(Entity actor)
		{
			SideComponent sideComponent = actor.GetComponent<SideComponent>();

			if (sideComponent != null)
			{
				List<Entity> actorList;

				if (actors.TryGetValue(sideComponent.Side, out actorList))
				{
					actorList.Add(actor);
				}
			}
		}

		private void RemoveSideActor(Entity actor)
		{
			SideComponent sideComponent = actor.GetComponent<SideComponent>();

			if (sideComponent != null)
			{
				List<Entity> actorList;

				if (actors.TryGetValue(sideComponent.Side, out actorList))
				{
					actorList.Remove(actor);
				}
			}
		}

		#endregion
	}
}
