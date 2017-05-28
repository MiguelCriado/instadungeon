using UnityEngine;

public class FollowMouse : MonoBehaviour
{
	private void Update()
	{
		Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		newPosition.z = 0;
		transform.position = newPosition;
	}
}
