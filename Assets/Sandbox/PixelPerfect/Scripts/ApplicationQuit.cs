using UnityEngine;

public class ApplicationQuit : MonoBehaviour
{
	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			Application.Quit();
		}
	}
}
