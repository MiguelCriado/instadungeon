using UnityEngine;
using UnityEngine.Events;

namespace InstaDungeon.Components
{
	public class TurnComponentEvent : UnityEvent<TurnComponent> { }

	public class TurnComponent : MonoBehaviour
	{
		public bool IsMyTurn { get { return token != null; } }
		public bool EntityCanAct { get { return IsMyTurn && token.EntityHasControl; } }
		public TurnToken Token { get { return token; } }
		public int Initiative { get { return initiative; } set { initiative = value; } }
		public int NumActions { get { return numActions; } set { numActions = value; } }

		public TurnComponentEvent OnTurnGranted = new TurnComponentEvent();
		public TurnComponentEvent OnTurnRevoked = new TurnComponentEvent();
		public TurnComponentEvent OnTurnDone = new TurnComponentEvent();

		[SerializeField] private int initiative;
		[SerializeField] private int numActions;

		private TurnToken token;

		protected int completedTurnActions;

		public void GrantTurn(TurnToken token)
		{
			this.token = token;
			completedTurnActions = 0;
			token.OnActionFinished.AddListener(OnActionDone);

			if (OnTurnGranted != null)
			{
				OnTurnGranted.Invoke(this);
			}
		}

		public void RevokeTurn(TurnToken token)
		{
			token.OnActionFinished.RemoveListener(OnActionDone);
			this.token = null;

			if (OnTurnRevoked != null)
			{
				OnTurnRevoked.Invoke(this);
			}
		}

		protected void OnActionDone()
		{
			completedTurnActions += 1;
			
			if (completedTurnActions >= numActions)
			{
				TurnDone();
			}
		}

		protected void TurnDone()
		{
			if (OnTurnDone != null)
			{
				OnTurnDone.Invoke(this);
			}
		}
	}
}
