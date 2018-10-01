using UnityEngine;

public class Rotate : MonoBehaviour
{
	public float degreesPerSecond = 360f;

	private void Update()
	{
		transform.Rotate(Vector3.forward, degreesPerSecond * Time.deltaTime);
	}
}
