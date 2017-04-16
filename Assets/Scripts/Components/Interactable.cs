using UnityEngine;

namespace InstaDungeon.Components
{
	[RequireComponent(typeof(Entity))]
	public class Interactable : MonoBehaviour
	{
		[SerializeField] protected Interaction interaction;

		private Entity entity;

		private void Awake()
		{
			entity = GetComponent<Entity>();
		}

		public bool IsValidInteraction(Entity actor)
		{
			return interaction.IsValidInteraction(actor, entity);
		}

		public void Interact(Entity actor, TurnToken token)
		{
			token.BufferAction(interaction.Interact(actor, entity));
		}
	}
}
