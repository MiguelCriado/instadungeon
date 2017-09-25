using InstaDungeon.Actions;
using InstaDungeon.Components;
using UnityEngine;

namespace InstaDungeon
{
	public abstract class Interaction : ScriptableObject
	{
		public int MinRange { get { return minRange; } }
		public int MaxRange { get { return maxRange; } }

		[SerializeField] private int minRange;
		[SerializeField] private int maxRange;

		public bool IsValidInteraction(Entity activeActor, Entity pasiveActor)
		{
			int manhattanDistance = int2.ManhattanDistance(activeActor.CellTransform.Position, pasiveActor.CellTransform.Position);

			return manhattanDistance >= MinRange && manhattanDistance <= MaxRange && IsValidInteractionInternal(activeActor, pasiveActor);
		}

		public abstract IAction Interact(Entity activeActor, Entity pasiveActor);

		protected abstract bool IsValidInteractionInternal(Entity activeActor, Entity pasiveActor);
	}
}
