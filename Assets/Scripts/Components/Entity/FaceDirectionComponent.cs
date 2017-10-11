using UnityEngine;

namespace InstaDungeon.Components
{
	public enum Direction
	{
		North,
		East,
		South,
		West
	}

	public class FaceDirectionComponent : MonoBehaviour
	{
		private static readonly string FacingDirectionId = "FacingDirection";

		public Direction Direction { get { return direction; } set { UpdateFacingDirection(value); } }

		[SerializeField] private Direction direction;

		private Animator animator;

		protected void Reset()
		{
			direction = Direction.North;
		}

		protected void Awake()
		{
			animator = GetComponent<Animator>();
			UpdateFacingDirection(direction, true);
		}

		protected void UpdateFacingDirection(Direction newDirection, bool forceUpdate = false)
		{
			if (newDirection != direction || forceUpdate)
			{
				if (animator != null)
				{
					animator.SetInteger(FacingDirectionId, (int)newDirection);
				}

				direction = newDirection;
			}
		}
	}
}
