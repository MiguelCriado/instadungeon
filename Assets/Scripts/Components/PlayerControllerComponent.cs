using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LocomotionComponent), typeof(TurnComponent))]
public class PlayerControllerComponent : MonoBehaviour
{
	[SerializeField]
	private KeyCode up = KeyCode.UpArrow;
	[SerializeField]
	private KeyCode right = KeyCode.RightArrow;
	[SerializeField]
	private KeyCode down = KeyCode.DownArrow;
	[SerializeField]
	private KeyCode left = KeyCode.LeftArrow;

	private LocomotionComponent locomotion;
	private TurnComponent turn;

	private UnityAction moveUp;
	private UnityAction moveRight;
	private UnityAction moveDown;
	private UnityAction moveLeft;

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
		if (turn.IsMyTurn)
		{
			UnityAction action = GetInput();

			if (action != null)
			{
				action.Invoke();
			}
		}
	}

	private UnityAction GetInput()
	{
		if (Input.GetKeyDown(up))
		{
			Debug.Log("Move up!!");
			return moveUp;
		}

		if (Input.GetKeyDown(right))
		{
			Debug.Log("Move right!!");
			return moveRight;
		}

		if (Input.GetKeyDown(down))
		{
			Debug.Log("Move down!!");
			return moveDown;
		}

		if (Input.GetKeyDown(left))
		{
			Debug.Log("Move left!!");
			return moveLeft;
		}

		return null;
	}

	private void TryMove(int x, int y)
	{
		if (locomotion.Move(x, y))
		{
			turn.TurnDone();
		}
	}
}
