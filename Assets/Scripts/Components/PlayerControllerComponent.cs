using UnityEngine;
using UnityEngine.Events;

namespace InstaDungeon.Components
{
	[RequireComponent(typeof(LocomotionComponent), typeof(TurnComponent))]
	public class PlayerControllerComponent : MonoBehaviour
	{
		protected static readonly float BufferedInputDecayTime = 0.5f;

		[SerializeField] private KeyCode up = KeyCode.UpArrow;
		[SerializeField] private KeyCode right = KeyCode.RightArrow;
		[SerializeField] private KeyCode down = KeyCode.DownArrow;
		[SerializeField] private KeyCode left = KeyCode.LeftArrow;

		private LocomotionComponent locomotion;
		private TurnComponent turn;

		private UnityAction moveUp;
		private UnityAction moveRight;
		private UnityAction moveDown;
		private UnityAction moveLeft;

		private UnityAction bufferedInput;
		private float bufferedeInputRemainingTime;

		void Awake()
		{
			locomotion = GetComponent<LocomotionComponent>();
			turn = GetComponent<TurnComponent>();

			moveUp = () => { TryMove(0, 1); };
			moveRight = () => { TryMove(1, 0); };
			moveDown = () => { TryMove(0, -1); };
			moveLeft = () => { TryMove(-1, 0); };
		}

		void Update()
		{
			BufferInput();

			if (turn.EntityCanAct)
			{
				UnityAction action = ConsumeBufferedInput();

				if (action != null)
				{
					action.Invoke();
				}
			}
		}

		private void BufferInput()
		{
			UnityAction action = GetInput();

			if (action != null)
			{
				bufferedInput = action;
				bufferedeInputRemainingTime = BufferedInputDecayTime;
			}
			
			if (bufferedInput != null)
			{
				bufferedeInputRemainingTime -= Time.deltaTime;

				if (bufferedeInputRemainingTime <= 0)
				{
					bufferedInput = null;
				}
			}
		}

		private UnityAction ConsumeBufferedInput()
		{
			UnityAction result = bufferedInput;
			bufferedInput = null;
			return result;
		}

		private UnityAction GetInput()
		{
			if (Input.GetKeyDown(up))
			{
				return moveUp;
			}

			if (Input.GetKeyDown(right))
			{
				return moveRight;
			}

			if (Input.GetKeyDown(down))
			{
				return moveDown;
			}

			if (Input.GetKeyDown(left))
			{
				return moveLeft;
			}

			return null;
		}

		private void TryMove(int x, int y)
		{
			if (locomotion.IsValidMovement(x, y))
			{
				locomotion.Move(new int2(x, y), turn.Token);
			}
		}
	}
}
