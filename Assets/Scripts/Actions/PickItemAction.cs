using DG.Tweening;
using InstaDungeon.Commands;
using InstaDungeon.Components;
using InstaDungeon.Models;

namespace InstaDungeon.Actions
{
	public class PickItemAction : BaseAction<PickItemCommand>
	{
		public PickItemAction(Entity actor, Entity item)
		{
			command = new PickItemCommand(actor, item);
			DOTween.Init();
		}

		public static bool IsValidInteraction(Entity actor, Entity item)
		{
			return actor.GetComponent<Inventory>() != null && item.GetComponent<Item>() != null;
		}

		public override void Act()
		{
			base.Act();

			// TODO drop and pick animation

			command.Execute();
			ActionDone();
		}
	}
}
