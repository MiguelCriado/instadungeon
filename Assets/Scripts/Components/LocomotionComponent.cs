using InstaDungeon.Commands;
using UnityEngine;

namespace InstaDungeon.Components
{
	[RequireComponent(typeof(CellTransform))]
	public class LocomotionComponent : MonoBehaviour
	{
		private CellTransform cellTransform;

		void Awake()
		{
			cellTransform = GetComponent<CellTransform>();
		}

		public bool Move(int xStep, int yStep)
		{
			return Move(new int2(xStep, yStep));
		}

		public bool Move(int2 step)
		{
			return GameManager.MoveActor(new MoveActorCommand(cellTransform, cellTransform.Position + step));
		}
	}
}
