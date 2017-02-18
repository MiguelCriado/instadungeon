using InstaDungeon.Actions;
using UnityEngine;

namespace InstaDungeon.Components
{
	[RequireComponent(typeof(Entity))]
	public class LocomotionComponent : MonoBehaviour
	{
		protected Entity entity;
		protected ActionManager actionManager;

		void Awake()
		{
			entity = GetComponent<Entity>();
			actionManager = Locator.Get<ActionManager>();
		}

		public bool IsValidMovement(int xStep, int yStep)
		{
			return IsValidMovement(new int2(xStep, yStep));
		}

		public bool IsValidMovement(int2 step)
		{
			return MoveEntityAction.IsValid(entity, entity.CellTransform.Position + step);
		}

		public void Move(int xStep, int yStep, TurnToken token)
		{
			Move(new int2(xStep, yStep), token);
		}

		public void Move(int2 step, TurnToken token)
		{
			MoveEntityAction action = new MoveEntityAction(entity, entity.CellTransform.Position + step);
			token.BufferAction(action);
		}
	}
}
