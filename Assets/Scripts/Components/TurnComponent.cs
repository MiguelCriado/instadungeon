using UnityEngine;
using UnityEngine.Events;

namespace InstaDungeon.Components
{
	public class TurnComponentEvent : UnityEvent<TurnComponent> { }

	public class TurnComponent : MonoBehaviour
	{
		public bool IsMyTurn { get { return token != null; } }
		public TurnToken Token { get { return token; } }
		public int Initiative { get { return initiative; } set { initiative = value; } }

		public TurnComponentEvent OnTurnGranted = new TurnComponentEvent();
		public TurnComponentEvent OnTurnRevoked = new TurnComponentEvent();
		public TurnComponentEvent OnTurnDone = new TurnComponentEvent();

		private TurnToken token;
		[SerializeField]
		private int initiative;

		public void GrantTurn(TurnToken token)
		{
			this.token = token;

			if (OnTurnGranted != null)
			{
				OnTurnGranted.Invoke(this);
			}
		}

		public void RevokeTurn(TurnToken token)
		{
			this.token = null;

			if (OnTurnRevoked != null)
			{
				OnTurnRevoked.Invoke(this);
			}
		}

		public void TurnDone()
		{
			if (OnTurnDone != null)
			{
				OnTurnDone.Invoke(this);
			}
		}
	}
}
