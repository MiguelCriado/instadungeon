using UnityEngine;

namespace InstaDungeon.Components
{
	public class SideComponent : MonoBehaviour
	{
		public ConflictSide Side { get { return side; } }

		[SerializeField] private ConflictSide side;
	}
}
