using InstaDungeon.Components;
using UnityEngine;

namespace InstaDungeon
{
	public abstract class Interaction : ScriptableObject
	{
		public abstract bool IsValidInteraction(Entity activeActor, Entity pasiveActor);
		public abstract void Interact(Entity activeActor, Entity pasiveActor);
	}
}
