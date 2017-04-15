using UnityEngine;

namespace InstaDungeon.Components
{
	[RequireComponent(typeof(Entity))]
	public class Interactable : MonoBehaviour
	{
		[SerializeField] protected Interaction interaction;

		private Entity entity;
		private ActionManager actionManager;

		private void Awake()
		{
			entity = GetComponent<Entity>();
			actionManager = Locator.Get<ActionManager>();
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
