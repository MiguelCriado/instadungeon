using DG.Tweening;
using InstaDungeon.Commands;
using InstaDungeon.Components;
using InstaDungeon.Models;

namespace InstaDungeon.Actions
{
	public class PickItemAction : BaseAction<PickItemCommand>
	{
		public PickItemAction(Entity actor, Item item)
		{
			command = new PickItemCommand(actor, item);
			DOTween.Init();
		}

		public static bool IsValidInteraction(Entity actor, Item item)
		{
			return actor.GetComponent<Inventory>() != null;
		}

		public override void Act()
		{
			base.Act();

			// TODO 
		}
	}
}
