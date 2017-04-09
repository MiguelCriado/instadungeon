using UnityEngine;
using UnityEngine.Events;

namespace InstaDungeon.Components
{
	[RequireComponent(typeof(TurnComponent), typeof(Actor))]
	public class InputActorController : MonoBehaviour
	{
		protected static readonly float BufferedInputDecayTime = 0.5f;

		[SerializeField] private KeyCode up;
		[SerializeField] private KeyCode right;
		[SerializeField] private KeyCode down;
		[SerializeField] private KeyCode left;

		private TurnComponent turn;
		private Actor actor;

		private UnityAction upAction;
		private UnityAction rightAction;
		private UnityAction downAction;
		private UnityAction leftAction;

		private UnityAction bufferedInput;
		private float bufferedeInputRemainingTime;

		private void Reset()
		{
			up = KeyCode.UpArrow;
			right = KeyCode.RightArrow;
			down = KeyCode.DownArrow;
			left = KeyCode.LeftArrow;
		}

		private void Awake()
		{
			turn = GetComponent<TurnComponent>();
			actor = GetComponent<Actor>();

			upAction = () => { actor.Up(); };
			rightAction = () => { actor.Right(); };
			downAction = () => { actor.Down(); };
			leftAction = () => { actor.Left(); };
		}

		private void Update()
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
				return upAction;
			}

			if (Input.GetKeyDown(right))
			{
				return rightAction;
			}

			if (Input.GetKeyDown(down))
			{
				return downAction;
			}

			if (Input.GetKeyDown(left))
			{
				return leftAction;
			}

			return null;
		}
	}
}
