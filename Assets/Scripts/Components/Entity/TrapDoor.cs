using UnityEngine;

namespace InstaDungeon.Components
{
	public class TrapDoor : MonoBehaviour
	{
		public bool IsOpen { get { return isOpen; } set { isOpen = value; } }

		[SerializeField] private bool isOpen;
	}
}
