using InstaDungeon.Components;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace InstaDungeon
{
	public class TurnEvent : UnityEvent<TurnToken> { }

	public class TurnManager
	{
		public bool TurnDone { get { return turnDone; } }
		public bool Running { get { return running; } }

		public TurnEvent OnRoundStarted = new TurnEvent();
		public TurnEvent OnRoundFinished = new TurnEvent();
		public TurnEvent OnTurnStarted = new TurnEvent();
		public TurnEvent OnTurnFinished = new TurnEvent();

		private List<TurnComponent> actors;
		private List<TurnComponent> pendingActors;
		private TurnToken token;
		private bool running;
		private bool turnDone;

		public TurnManager()
		{
			actors = new List<TurnComponent>();
			pendingActors = new List<TurnComponent>();
			token = new TurnToken();
		}

		public void AddActor(TurnComponent actor)
		{
			pendingActors.Add(actor);
		}

		public void AddActor(GameObject actor)
		{
			TurnComponent actorsTurn = actor.GetComponent<TurnComponent>();

			if (actorsTurn != null)
			{
				AddActor(actorsTurn);
			}
		}

		public void RemoveActor(TurnComponent actor)
		{
			actors.Remove(actor);
			pendingActors.Remove(actor);
		}

		public void Init()
		{
			token.Round = 0;
			token.Turn = 0;
			running = true;
			turnDone = true;
		}

		public void GrantControl()
		{
			token.GrantControl();
		}

		public void RevokeControl()
		{
			token.RevokeControl();
		}

		public void StartRound()
		{
			if (pendingActors.Count > 0)
			{
				actors.AddRange(pendingActors);
				actors.Sort((a, b) => a.Initiative.CompareTo(b.Initiative));

				pendingActors.Clear();
			}

			token.Turn = 0;
			turnDone = false;

			if (actors.Count > 0)
			{
				GrantTurn(actors[0]);

				if (OnRoundStarted != null)
				{
					OnRoundStarted.Invoke(token);
				}
			}
		}

		public void UpdateTurn()
		{
			if (running && turnDone)
			{
				turnDone = false;

				NextTurn();
			}
		}

		public void Update()
		{
			if (running)
			{
				token.RunBufferedAction();
			}
		}

		private void NextTurn()
		{
			if (actors.Count > token.Turn + 1)
			{
				if (OnTurnFinished != null)
				{
					OnTurnFinished.Invoke(token);
				}

				token.Turn++;
				turnDone = false;
				GrantTurn(actors[token.Turn]);
			}
			else
			{
				if (OnRoundFinished != null)
				{
					OnRoundFinished.Invoke(token);
				}

				token.Target = null;
				token.Round++;
				StartRound();
			}
		}

		private void GrantTurn(TurnComponent actor)
		{
			token.Target = actor;
			actor.OnTurnDone.AddListener(CurrentTurnDone);
			actor.GrantTurn(token);

			if (OnTurnStarted != null)
			{
				OnTurnStarted.Invoke(token);
			}
		}

		private void CurrentTurnDone(TurnComponent component)
		{
			actors[token.Turn].RevokeTurn(token);
			actors[token.Turn].OnTurnDone.RemoveListener(CurrentTurnDone);
			turnDone = true;
		}
	}
}
