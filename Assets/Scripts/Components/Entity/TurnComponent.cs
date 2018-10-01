using InstaDungeon.Events;
using UnityEngine;
using UnityEngine.Events;

namespace InstaDungeon.Components
{
	public class TurnComponentEvent : UnityEvent<TurnComponent> { }

	[RequireComponent(typeof(Entity))]
	public class TurnComponent : MonoBehaviour
	{
		public Entity Entity { get { return entity; } }
		public bool IsMyTurn { get { return token != null; } }
		public bool EntityCanAct { get { return IsMyTurn && token.EntityHasControl; } }
		public TurnToken Token { get { return token; } }
		public int Initiative { get { return initiative; } set { initiative = value; } }
		public int NumActions { get { return numActions; } set { numActions = value; } }

		[SerializeField] private int initiative;
		[SerializeField] private int numActions;

		private Entity entity;
		private TurnToken token;
		private int completedTurnActions;

		private void Awake()
		{
			entity = GetComponent<Entity>();
		}

		public void GrantTurn(TurnToken token)
		{
			this.token = token;
			completedTurnActions = 0;
			token.OnActionFinished.AddListener(OnActionDone);
			entity.Events.TriggerEvent(new EntityGrantTurnEvent(entity));
		}

		public void RevokeTurn(TurnToken token)
		{
			token.OnActionFinished.RemoveListener(OnActionDone);
			this.token = null;
			entity.Events.TriggerEvent(new EntityRevokeTurnEvent(entity));
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
			entity.Events.TriggerEvent(new EntityTurnDoneEvent(entity));
		}
	}
}
