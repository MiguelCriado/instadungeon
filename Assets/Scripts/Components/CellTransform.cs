using UnityEngine;

public class CellTransform : MonoBehaviour
{
	public int2 Position { get; set; }

	[SerializeField]
	private int2 position;

	public void MoveTo(int2 position)
	{
		GameManager.MoveActorTo(gameObject, position);
	}
}
